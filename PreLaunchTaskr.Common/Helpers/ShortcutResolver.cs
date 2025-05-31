using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices; // For Guid

namespace PreLaunchTaskr.Common.Helpers;

public static class ShortcutResolver
{
    // Based on MS-SHLLINK: Shell Link (.LNK) Binary File Format

    [Flags]
    private enum LinkFlags : uint
    {
        HasLinkTargetIDList = 0x00000001,
        HasLinkInfo = 0x00000002,
        HasName = 0x00000004,
        HasRelativePath = 0x00000008,
        HasWorkingDirectory = 0x00000010,
        HasArguments = 0x00000020,
        HasIconLocation = 0x00000040,
        IsUnicode = 0x00000080,
        ForceNoLinkInfo = 0x00000100,
        HasExpString = 0x00000200, // Often referred to as HasEnvironmentVariableDataBlock
        RunInSeparateProcess = 0x00000400,
        Reserved0 = 0x00000800, // Not used
        HasDarwinID = 0x00001000,
        RunAsUser = 0x00002000,
        HasExpIcon = 0x00004000,
        NoPidlAlias = 0x00008000,
        Reserved1 = 0x00010000, // Not used
        RunWithShimLayer = 0x00020000,
        ForceNoLinkTrack = 0x00040000,
        EnableTargetMetadata = 0x00080000,
        DisableLinkPathTracking = 0x00100000,
        DisableKnownFolderTracking = 0x00200000,
        DisableKnownFolderAlias = 0x00400000,
        AllowLinkToLink = 0x00800000,
        UnaliasOnSave = 0x01000000,
        PreferEnvironmentPath = 0x02000000,
        KeepLocalIDListForUNCTarget = 0x04000000
    }

    [Flags]
    private enum LinkInfoFlags : uint
    {
        VolumeIDAndLocalBasePath = 0x00000001,
        CommonNetworkRelativeLinkAndPathSuffix = 0x00000002
    }

    // EnvironmentVariableDataBlock signature
    private const uint ENV_VAR_SIG = 0xA0000003;

    public static string GetPathFromShortcut(string shortcutPath)
    {
        if (string.IsNullOrEmpty(shortcutPath))
            throw new ArgumentNullException(nameof(shortcutPath));
        if (!File.Exists(shortcutPath))
            throw new FileNotFoundException("Shortcut file not found.", shortcutPath);
        if (!Path.GetExtension(shortcutPath).Equals(".lnk", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("File is not a .lnk file.", nameof(shortcutPath));

        using (FileStream fs = new FileStream(shortcutPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        using (BinaryReader reader = new BinaryReader(fs, Encoding.Default)) // Default encoding for ANSI strings
        {
            // --- ShellLinkHeader (76 bytes) ---
            // HeaderSize (4 bytes): Must be 0x0000004C
            uint headerSize = reader.ReadUInt32();
            if (headerSize != 0x4C)
                throw new FormatException("Invalid LNK file: Incorrect header size.");

            // LinkCLSID (16 bytes): Must be {00021401-0000-0000-C000-000000000046}
            Guid linkClsid = new Guid(reader.ReadBytes(16));
            if (linkClsid != new Guid("00021401-0000-0000-C000-000000000046"))
                throw new FormatException("Invalid LNK file: Incorrect CLSID.");

            // LinkFlags (4 bytes)
            LinkFlags linkFlags = (LinkFlags)reader.ReadUInt32();

            // FileAttributes (4 bytes) - Attributes of the target
            // CreationTime (8 bytes) - FILETIME of target
            // AccessTime (8 bytes) - FILETIME of target
            // WriteTime (8 bytes) - FILETIME of target
            // FileSize (4 bytes) - Size of target
            // IconIndex (4 bytes) - Icon index for target
            // ShowCommand (4 bytes) - e.g., SW_SHOWNORMAL
            // HotKey (2 bytes)
            // Reserved1 (2 bytes)
            // Reserved2 (4 bytes)
            // Reserved3 (4 bytes)
            fs.Seek(4 + 8 + 8 + 8 + 4 + 4 + 4 + 2 + 2 + 4 + 4, SeekOrigin.Current); // Skip these 40 bytes

            string targetPath = null;
            string relativePathString = null;
            string workingDirectoryString = null;

            // --- LinkTargetIDList (optional) ---
            if ((linkFlags & LinkFlags.HasLinkTargetIDList) != 0)
            {
                ushort idListSize = reader.ReadUInt16();
                // For simplicity, we are skipping the actual parsing of IDList.
                // It can be complex, and LinkInfo often provides the path more directly for file/folder targets.
                fs.Seek(idListSize, SeekOrigin.Current);
            }

            // --- LinkInfo (optional, but usually present if HasLinkInfo is set) ---
            long linkInfoStartOffset = -1;
            if ((linkFlags & LinkFlags.HasLinkInfo) != 0)
            {
                linkInfoStartOffset = fs.Position;
                uint linkInfoSize = reader.ReadUInt32();
                uint linkInfoHeaderSize = reader.ReadUInt32(); // Should be >= 0x1C (28)
                LinkInfoFlags linkInfoFlags = (LinkInfoFlags)reader.ReadUInt32();
                uint volumeIdOffset = reader.ReadUInt32();
                uint localBasePathOffset = reader.ReadUInt32();
                uint commonNetworkRelativeLinkOffset = reader.ReadUInt32();
                uint commonPathSuffixOffset = reader.ReadUInt32();
                
                uint localBasePathOffsetUnicode = 0;
                uint commonPathSuffixOffsetUnicode = 0;

                if (linkInfoHeaderSize >= 0x24) // 36 bytes, means Unicode offsets are present
                {
                    localBasePathOffsetUnicode = reader.ReadUInt32();
                    commonPathSuffixOffsetUnicode = reader.ReadUInt32();
                }

                // Extract LocalBasePath
                if ((linkInfoFlags & LinkInfoFlags.VolumeIDAndLocalBasePath) != 0)
                {
                    if (localBasePathOffsetUnicode > 0 && linkInfoStartOffset + localBasePathOffsetUnicode < linkInfoStartOffset + linkInfoSize)
                    {
                        fs.Seek(linkInfoStartOffset + localBasePathOffsetUnicode, SeekOrigin.Begin);
                        targetPath = ReadNullTerminatedUnicodeString(reader);
                    }
                    else if (localBasePathOffset > 0 && linkInfoStartOffset + localBasePathOffset < linkInfoStartOffset + linkInfoSize)
                    {
                        fs.Seek(linkInfoStartOffset + localBasePathOffset, SeekOrigin.Begin);
                        targetPath = ReadNullTerminatedAnsiString(reader);
                    }
                }

                // Extract CommonNetworkRelativeLink and CommonPathSuffix (if LocalBasePath wasn't found or if preferred)
                if (string.IsNullOrEmpty(targetPath) && (linkInfoFlags & LinkInfoFlags.CommonNetworkRelativeLinkAndPathSuffix) != 0)
                {
                    // According to MS-SHLLINK, CommonNetworkRelativeLink contains a CommonNetworkRelativeLink structure.
                    // The PathSuffix is appended to the DeviceName from that structure.
                    // This is more complex, for now, we prioritize LocalBasePath which often holds the full UNC path too.
                    // A simple concatenation (if both offsets are valid):
                    string networkPart = null;
                    string suffixPart = null;

                    if (commonNetworkRelativeLinkOffset > 0 && linkInfoStartOffset + commonNetworkRelativeLinkOffset < linkInfoStartOffset + linkInfoSize)
                    {
                        fs.Seek(linkInfoStartOffset + commonNetworkRelativeLinkOffset, SeekOrigin.Begin);
                        // The CommonNetworkRelativeLink structure itself contains more offsets.
                        // For a simplified approach, let's assume it might directly point to a string for some cases or we read what's there.
                        // This part requires deeper parsing of CommonNetworkRelativeLink structure.
                        // A robust parser would read the CommonNetworkRelativeLink structure, then its DeviceName.
                        // For now, let's try to read a string if available there, assuming it's just the path.
                        // THIS IS A SIMPLIFICATION and might not work for all network links.
                        // True parsing would involve reading CommonNetworkRelativeLink.NetworkProviderType and then
                        // the strings based on whether NetNameOffset > 0x14 and DeviceNameOffset > 0x14.
                        
                        // For now, if LocalBasePath failed, this is a fallback, let's assume the string is right there.
                        // This part is tricky because CommonNetworkRelativeLinkOffset points to a structure, not directly a string.
                        // Let's assume for now that if LocalBasePath is empty, this might be a simple UNC.
                        // fs.Seek(linkInfoStartOffset + commonNetworkRelativeLinkOffset, SeekOrigin.Begin);
                        // networkPart = ReadNullTerminatedAnsiString(reader); // Or Unicode if available
                    }
                    
                    // Suffix part is more straightforward
                    if (commonPathSuffixOffset > 0 && linkInfoStartOffset + commonPathSuffixOffset < linkInfoStartOffset + linkInfoSize)
                    {
                       fs.Seek(linkInfoStartOffset + commonPathSuffixOffset, SeekOrigin.Begin);
                       suffixPart = ReadNullTerminatedAnsiString(reader); // Or Unicode. MS-SHLLINK implies these are null-terminated.
                    }
                    
                    // if (!string.IsNullOrEmpty(networkPart) && !string.IsNullOrEmpty(suffixPart))
                    // {
                    //    targetPath = Path.Combine(networkPart, suffixPart); // Path.Combine might not be right for UNC parts.
                    // } else if (!string.IsNullOrEmpty(networkPart)) {
                    //    targetPath = networkPart;
                    // }

                    // Correct path to ensure we are at the end of LinkInfo block
                    fs.Seek(linkInfoStartOffset + linkInfoSize, SeekOrigin.Begin);
                }
                else
                {
                    // Ensure we are at the end of LinkInfo block if we didn't seek for network paths
                     fs.Seek(linkInfoStartOffset + linkInfoSize, SeekOrigin.Begin);
                }
            }


            // --- StringData (Name, RelativePath, WorkingDirectory, Arguments, IconLocation) ---
            // These are length-prefixed strings.
            bool isUnicode = (linkFlags & LinkFlags.IsUnicode) != 0;

            if ((linkFlags & LinkFlags.HasName) != 0)
            {
                // Skip Name string for path extraction
                ReadLengthPrefixedString(reader, isUnicode);
            }

            if ((linkFlags & LinkFlags.HasRelativePath) != 0)
            {
                relativePathString = ReadLengthPrefixedString(reader, isUnicode);
            }

            if ((linkFlags & LinkFlags.HasWorkingDirectory) != 0)
            {
                workingDirectoryString = ReadLengthPrefixedString(reader, isUnicode);
            }

            if ((linkFlags & LinkFlags.HasArguments) != 0)
            {
                // Skip Arguments string
                ReadLengthPrefixedString(reader, isUnicode);
            }

            if ((linkFlags & LinkFlags.HasIconLocation) != 0)
            {
                // Skip IconLocation string
                ReadLengthPrefixedString(reader, isUnicode);
            }
            
            // --- ExtraData ---
            // Look for EnvironmentVariableDataBlock if HasExpString is set
            // And only if we haven't found a good path yet, or if PreferEnvironmentPath is set.
            // PreferEnvironmentPath is not directly checked here, but if HasExpString is set, it's a good candidate.

            string envPath = null;
            if ((linkFlags & LinkFlags.HasExpString) != 0)
            {
                while (fs.Position < fs.Length)
                {
                    uint blockSize = reader.ReadUInt32();
                    if (blockSize < 4) break; // Terminal block or error

                    uint blockSignature = reader.ReadUInt32();
                    long nextBlockPos = fs.Position + blockSize - 8; // -8 for size and sig already read

                    if (blockSignature == ENV_VAR_SIG) // EnvironmentVariableDataBlock
                    {
                        // TargetAnsi (260 bytes, fixed size buffer, null-terminated)
                        byte[] targetAnsiBytes = reader.ReadBytes(260);
                        string ansiPath = Encoding.Default.GetString(targetAnsiBytes).Split('\0')[0];

                        // TargetUnicode (520 bytes, fixed size buffer, null-terminated)
                        byte[] targetUnicodeBytes = reader.ReadBytes(520);
                        string unicodePath = Encoding.Unicode.GetString(targetUnicodeBytes).Split('\0')[0];
                        
                        envPath = !string.IsNullOrEmpty(unicodePath) ? unicodePath : ansiPath;
                        if (!string.IsNullOrEmpty(envPath))
                        {
                             // Environment path often takes precedence or is the primary one for some shortcuts
                            targetPath = Environment.ExpandEnvironmentVariables(envPath);
                        }
                        break; // Found it, no need to parse further ExtraData blocks for path
                    }
                    
                    if (nextBlockPos > fs.Length) break; // Avoid overruns
                    fs.Seek(nextBlockPos, SeekOrigin.Begin);
                }
            }

            // If LinkInfo didn't provide an absolute path, and we have a relative path, try to resolve it.
            if (string.IsNullOrEmpty(targetPath) && !string.IsNullOrEmpty(relativePathString))
            {
                if (!string.IsNullOrEmpty(workingDirectoryString))
                {
                    // Relative to working directory
                    targetPath = Path.Combine(workingDirectoryString, relativePathString);
                }
                else
                {
                    // Relative to the LNK file's directory
                    targetPath = Path.Combine(Path.GetDirectoryName(shortcutPath), relativePathString);
                }
                targetPath = Path.GetFullPath(targetPath); // Normalize
            }

            return targetPath;
        }
    }

    private static string ReadNullTerminatedAnsiString(BinaryReader reader)
    {
        var sb = new StringBuilder();
        byte b;
        while ((b = reader.ReadByte()) != 0)
        {
            sb.Append((char)b); // Assumes system default ANSI codepage
        }
        return sb.ToString();
    }

    private static string ReadNullTerminatedUnicodeString(BinaryReader reader)
    {
        var sb = new StringBuilder();
        char c;
        while (true)
        {
            byte b1 = reader.ReadByte();
            byte b2 = reader.ReadByte();
            if (b1 == 0 && b2 == 0) break;
            c = (char)((b2 << 8) | b1); // UTF-16 LE
            sb.Append(c);
        }
        return sb.ToString();
    }

    private static string ReadLengthPrefixedString(BinaryReader reader, bool isUnicode)
    {
        // CountCharacters (2 bytes) - Number of characters in the string.
        ushort charCount = reader.ReadUInt16();
        if (charCount == 0) return string.Empty;

        byte[] buffer;
        if (isUnicode)
        {
            buffer = reader.ReadBytes(charCount * 2);
            return Encoding.Unicode.GetString(buffer);
        }
        else
        {
            buffer = reader.ReadBytes(charCount);
            return Encoding.Default.GetString(buffer); // System's default ANSI code page
        }
    }
}