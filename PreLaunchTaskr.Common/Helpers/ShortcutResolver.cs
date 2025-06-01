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
        HasExpString = 0x00000200,
        RunInSeparateProcess = 0x00000400,
        Reserved0 = 0x00000800,
        HasDarwinID = 0x00001000,
        RunAsUser = 0x00002000,
        HasExpIcon = 0x00004000,
        NoPidlAlias = 0x00008000,
        Reserved1 = 0x00010000,
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

    private const uint ENV_VAR_SIG = 0xA0000003;
    private const uint ITEM_ID_EXTENSION_SIGNATURE_BEEF0004 = 0xBEEF0004; 

    public static string? GetPathFromShortcut(string shortcutPath)
    {
        if (string.IsNullOrEmpty(shortcutPath))
            throw new ArgumentNullException(nameof(shortcutPath));

        string resolvedShortcutPath = Environment.ExpandEnvironmentVariables(shortcutPath);

        if (!File.Exists(resolvedShortcutPath))
            throw new FileNotFoundException("Shortcut file not found.", resolvedShortcutPath);
        if (!Path.GetExtension(resolvedShortcutPath).Equals(".lnk", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("File is not a .lnk file.", nameof(shortcutPath));

        using (FileStream fs = new(resolvedShortcutPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        using (BinaryReader reader = new(fs, Encoding.Default)) 
        {
            // --- ShellLinkHeader ---
            uint headerSize = reader.ReadUInt32();
            if (headerSize != 0x4C)
                throw new FormatException("Invalid LNK file: Incorrect header size.");

            Guid linkClsid = new Guid(reader.ReadBytes(16));
            if (linkClsid != new Guid("00021401-0000-0000-C000-000000000046"))
                throw new FormatException("Invalid LNK file: Incorrect CLSID.");

            LinkFlags linkFlags = (LinkFlags)reader.ReadUInt32();
            fs.Seek(52, SeekOrigin.Current); // Skip rest of header (FileAttributes to Reserved3)

            string? targetPath = null;
            string? relativePathString = null;
            string? workingDirectoryString = null;
            string? lastNameFromPidl = null;

            // --- LinkTargetIDList ---
            if ((linkFlags & LinkFlags.HasLinkTargetIDList) != 0)
            {
                long idListStartPos = fs.Position;
                if (idListStartPos + 2 > fs.Length) throw new FormatException("Invalid LNK: Unexpected EOF reading IDList size.");
                ushort idListSize = reader.ReadUInt16();

                if (idListSize > 0)
                {
                    if (idListStartPos + 2 + idListSize <= fs.Length)
                    {
                        lastNameFromPidl = ExtractLastNameFromItemIDList(reader, idListSize);
                        // Ensure reader is at the end of IDList block
                        fs.Seek(idListStartPos + 2 + idListSize, SeekOrigin.Begin);
                    }
                    else 
                    {
                        throw new FormatException("Invalid LNK: IDList size exceeds file length.");
                    }
                }
            }

            // --- LinkInfo ---
            if ((linkFlags & LinkFlags.HasLinkInfo) != 0 && (linkFlags & LinkFlags.ForceNoLinkInfo) == 0)
            {
                long linkInfoStartOffset = fs.Position;
                if (linkInfoStartOffset + 4 > fs.Length) throw new FormatException("Invalid LNK: Unexpected EOF reading LinkInfo size.");
                uint linkInfoSize = reader.ReadUInt32();

                if (linkInfoSize >= 28) // Minimum size for LinkInfo header (LinkInfoSize to CommonPathSuffixOffset)
                {
                    if (linkInfoStartOffset + linkInfoSize <= fs.Length)
                    {
                        uint linkInfoHeaderSize = reader.ReadUInt32(); 
                        if (linkInfoHeaderSize < 28) throw new FormatException("Invalid LNK: LinkInfo header size too small for mandatory fields.");

                        LinkInfoFlags linkInfoFlagsVal = (LinkInfoFlags)reader.ReadUInt32();
                        // These next 4 offsets are within the first 28 bytes of LinkInfo block (after LinkInfoSize and LinkInfoHeaderSize)
                        uint volumeIdOffset = reader.ReadUInt32();
                        uint localBasePathOffset = reader.ReadUInt32();
                        uint commonNetworkRelativeLinkOffset = reader.ReadUInt32();
                        uint commonPathSuffixOffset = reader.ReadUInt32();
                        
                        uint localBasePathOffsetUnicode = 0;

                        if (linkInfoHeaderSize >= 0x24) // Header is 36 bytes or more, includes Unicode offsets
                        {
                            // We've read 28 bytes of the LinkInfo block's content (4 for LinkInfoHeaderSize, 24 for flags and first 4 offsets).
                            // The next 8 bytes (for Unicode offsets) should be present if linkInfoHeaderSize >= 36 (0x24).
                            // Check if enough bytes remain in the *file stream* for these.
                            if (fs.Position + 8 > fs.Length) throw new FormatException("Invalid LNK: Unexpected EOF reading LinkInfo Unicode offsets.");
                            localBasePathOffsetUnicode = reader.ReadUInt32();
                            fs.Seek(4, SeekOrigin.Current); // Skip CommonPathSuffixOffsetUnicode as it's not used for path construction here
                        }

                        if ((linkInfoFlagsVal & LinkInfoFlags.VolumeIDAndLocalBasePath) != 0)
                        {
                            // Prioritize Unicode path
                            if (localBasePathOffsetUnicode > 0 && 
                                localBasePathOffsetUnicode < linkInfoSize && // Offset must be within LinkInfo block data
                                linkInfoStartOffset + localBasePathOffsetUnicode < fs.Length) // Read must be within file bounds
                            {
                                fs.Seek(linkInfoStartOffset + localBasePathOffsetUnicode, SeekOrigin.Begin);
                                targetPath = ReadNullTerminatedUnicodeString(reader);
                            }
                            // Fallback to ANSI path
                            else if (localBasePathOffset > 0 &&
                                     localBasePathOffset < linkInfoSize &&
                                     linkInfoStartOffset + localBasePathOffset < fs.Length)
                            {
                                fs.Seek(linkInfoStartOffset + localBasePathOffset, SeekOrigin.Begin);
                                targetPath = ReadNullTerminatedAnsiString(reader);
                            }
                        }
                        // Ensure reader is positioned at the end of the LinkInfo block
                        fs.Seek(linkInfoStartOffset + linkInfoSize, SeekOrigin.Begin);
                    }
                    else 
                    {
                        throw new FormatException("Invalid LNK file: LinkInfo size exceeds file length.");
                    }
                }
                else if (linkInfoSize > 0) // LinkInfoSize is non-zero but too small for mandatory header
                {
                     throw new FormatException("Invalid LNK file: LinkInfo block size is too small.");
                }
            }

            // --- StringData ---
            bool isUnicode = (linkFlags & LinkFlags.IsUnicode) != 0;
            if ((linkFlags & LinkFlags.HasName) != 0) ReadLengthPrefixedString(reader, isUnicode);
            if ((linkFlags & LinkFlags.HasRelativePath) != 0) relativePathString = ReadLengthPrefixedString(reader, isUnicode);
            if ((linkFlags & LinkFlags.HasWorkingDirectory) != 0) workingDirectoryString = ReadLengthPrefixedString(reader, isUnicode);
            if ((linkFlags & LinkFlags.HasArguments) != 0) ReadLengthPrefixedString(reader, isUnicode); 
            if ((linkFlags & LinkFlags.HasIconLocation) != 0) ReadLengthPrefixedString(reader, isUnicode);
            
            // --- ExtraData ---
            if ((linkFlags & LinkFlags.HasExpString) != 0)
            {
                string? envBlockPath = null;
                while (fs.Position <= fs.Length - 8) // Min 8 bytes for BlockSize & BlockSignature
                {
                    long currentBlockHeaderPos = fs.Position;
                    uint blockSize = reader.ReadUInt32();

                    if (blockSize == 0) break; // Terminal block (MS-SHLLINK 2.5)
                    if (blockSize < 8) throw new FormatException("Invalid LNK: ExtraData block size too small."); 

                    uint blockSignature = reader.ReadUInt32();
                    
                    long nextBlockHeaderPos = currentBlockHeaderPos + blockSize;
                    if (nextBlockHeaderPos > fs.Length || nextBlockHeaderPos <= currentBlockHeaderPos)
                        throw new FormatException("Invalid LNK: ExtraData block size causes invalid seek or goes beyond EOF.");

                    if (blockSignature == ENV_VAR_SIG)
                    {
                        // EnvironmentVariableDataBlock (MS-SHLLINK 2.5.4)
                        // Data: TargetAnsi (260) + TargetUnicode (520) = 780 bytes
                        // Total BlockSize must be at least 8 (header) + 780 (data) = 788 bytes
                        if (blockSize >= (260 + 520 + 8)) 
                        {
                            // Check if there's enough data *within this block's declared size* for the paths
                            if (fs.Position + 260 + 520 <= nextBlockHeaderPos) 
                            {
                                byte[] targetAnsiBytes = reader.ReadBytes(260);
                                string ansiPath = Encoding.Default.GetString(targetAnsiBytes).Split('\0')[0];
                                byte[] targetUnicodeBytes = reader.ReadBytes(520);
                                string unicodePath = Encoding.Unicode.GetString(targetUnicodeBytes).Split('\0')[0];
                                envBlockPath = !string.IsNullOrEmpty(unicodePath) ? unicodePath : ansiPath;
                            } 
                            // else: Block claims to be ENV_VAR_SIG and large enough overall, but not enough data remaining up to nextBlockHeaderPos. Malformed.
                        }
                        // else: ENV_VAR_SIG found, but block size is too small for its defined content. Malformed.

                        if (!string.IsNullOrEmpty(envBlockPath))
                        {
                            targetPath = Environment.ExpandEnvironmentVariables(envBlockPath);
                        }
                        fs.Seek(nextBlockHeaderPos, SeekOrigin.Begin);
                        break; // Found and processed (or attempted to process) ENV_VAR_SIG block
                    }
                    else
                    {
                        fs.Seek(nextBlockHeaderPos, SeekOrigin.Begin); // Skip to next block
                    }
                }
            }

            // --- Path Resolution Logic ---
            // Priority: 1. EnvVar, 2. LinkInfo, 3. PIDL+WorkingDir, 4. RelativePath+WorkingDir

            if (string.IsNullOrEmpty(targetPath) && !string.IsNullOrEmpty(lastNameFromPidl))
            {
                string? basePathForPidl = !string.IsNullOrEmpty(workingDirectoryString)
                    ? workingDirectoryString
                    : Path.GetDirectoryName(resolvedShortcutPath); 

                if (!string.IsNullOrEmpty(basePathForPidl))
                {
                    basePathForPidl = Environment.ExpandEnvironmentVariables(basePathForPidl);
                    targetPath = Path.Combine(basePathForPidl, lastNameFromPidl);
                }
                else 
                {
                    // Should not happen if resolvedShortcutPath is valid, but as a fallback:
                    targetPath = lastNameFromPidl; // Treat as relative to current dir if no base path
                }
            }

            if (string.IsNullOrEmpty(targetPath) && !string.IsNullOrEmpty(relativePathString))
            {
                string? basePathForRelative = !string.IsNullOrEmpty(workingDirectoryString)
                    ? workingDirectoryString
                    : Path.GetDirectoryName(resolvedShortcutPath);

                if (!string.IsNullOrEmpty(basePathForRelative))
                {
                     basePathForRelative = Environment.ExpandEnvironmentVariables(basePathForRelative);
                     targetPath = Path.Combine(basePathForRelative, relativePathString);
                }
                else
                {
                    targetPath = relativePathString;
                }
            }
            
            // Final normalization and environment variable expansion on the combined path
            if (!string.IsNullOrEmpty(targetPath))
            {
                try
                {
                    // Expand vars first, then get full path
                    targetPath = Environment.ExpandEnvironmentVariables(targetPath);
                    targetPath = Path.GetFullPath(targetPath);
                }
                catch (Exception) // ArgumentException, PathTooLongException, NotSupportedException etc.
                {
                    // Path could not be normalized or is invalid. Keep the combined path as is.
                    // Caller might need to handle this potentially problematic path.
                }
            }

            return targetPath;
        }
    }

    private static string? ExtractLastNameFromItemIDList(BinaryReader reader, ushort idListSize)
    {
        string? lastName = null; 
        long idListStartStreamPos = reader.BaseStream.Position; // Position in the main file stream
        long idListEndStreamPos = idListStartStreamPos + idListSize;
        
        while(reader.BaseStream.Position + 2 <= idListEndStreamPos) // Ensure can read ItemIDSize (2 bytes)
        {
            ushort itemIDSize = reader.ReadUInt16();
            if (itemIDSize == 0) break; // End of IDList (Terminal ItemID)
            if (itemIDSize < 2) throw new FormatException("Invalid LNK: ItemID size too small."); 

            long currentItemIDDataStartPos = reader.BaseStream.Position;
            // Calculate where the data for this ItemID ends in the main stream.
            // This is also where the next ItemID's size field would start if this wasn't the last one.
            long currentItemIDDataEndPos = currentItemIDDataStartPos + itemIDSize - 2; 

            if (currentItemIDDataEndPos > idListEndStreamPos) 
                throw new FormatException("Invalid LNK: ItemID size exceeds remaining IDList length.");

            byte[] itemData = reader.ReadBytes(itemIDSize - 2); // Read ItemID.Data from main stream
            
            using (MemoryStream msItem = new MemoryStream(itemData))
            using (BinaryReader brItem = new BinaryReader(msItem, Encoding.Default)) // Default for ANSI parts
            {
                string? currentItemName = null; 
                string? initialAnsiName = null; 

                // Heuristic 1: Try reading a primary name (often short name, ANSI) from start of ItemID.Data
                if (brItem.BaseStream.Length > 0)
                {
                    brItem.BaseStream.Position = 0; // Ensure position is at start of MemoryStream
                    initialAnsiName = ReadNullTerminatedAnsiString(brItem);
                    if (!string.IsNullOrEmpty(initialAnsiName))
                    {
                        currentItemName = initialAnsiName;
                    }
                }
                
                // Heuristic 2: Attempt to find and parse 0xBEEF0004 extension block
                const int extBlockOffsetInData = 0x0E; // Typical offset of extension block within ItemID.Data
                if (itemData.Length >= extBlockOffsetInData + 4) // Enough space to reach offset and read 4-byte signature
                {
                    brItem.BaseStream.Position = extBlockOffsetInData; // Seek to potential start of signature in MemoryStream
                    uint extSignature = brItem.ReadUInt32(); // Read the 4-byte signature

                    if (extSignature == ITEM_ID_EXTENSION_SIGNATURE_BEEF0004)
                    {
                        string? longNameFromExt = null;
                        string? shortNameFromExt = null;

                        // Try to read Long Unicode Name from extension block (MS-SHLLINK 2.2.2.1)
                        const int longNameOffsetInExtBlock = 0x1C; // Offset from start of extension block
                        long absLongNameOffsetInItemData = extBlockOffsetInData + longNameOffsetInExtBlock;
                        if (absLongNameOffsetInItemData < itemData.Length) // Check if offset is within itemData bounds
                        {
                            brItem.BaseStream.Position = absLongNameOffsetInItemData;
                            longNameFromExt = ReadNullTerminatedUnicodeString(brItem);
                        }
                        
                        // Try to read Short ANSI Name from extension block
                        const int shortNameOffsetInExtBlock = 0x0E; // Offset from start of extension block
                        long absShortNameOffsetInItemData = extBlockOffsetInData + shortNameOffsetInExtBlock;
                        if (absShortNameOffsetInItemData < itemData.Length)
                        {
                             brItem.BaseStream.Position = absShortNameOffsetInItemData;
                             shortNameFromExt = ReadNullTerminatedAnsiString(brItem);
                        }

                        // Apply priority: Long Unicode > Short ANSI from Ext > Initial ANSI guess
                        if (!string.IsNullOrEmpty(longNameFromExt))
                        {
                            currentItemName = longNameFromExt;
                        }
                        else if (!string.IsNullOrEmpty(shortNameFromExt))
                        {
                            currentItemName = shortNameFromExt;
                        }
                        // If both from extension block are null/empty, currentItemName retains initialAnsiName (if any)
                    }
                } 
                
                if (!string.IsNullOrEmpty(currentItemName))
                {
                    lastName = currentItemName; // Update with name from this (potentially last) ItemID
                }
            }
            // Crucial: Position the main file stream reader to the end of the current ItemID's data
            reader.BaseStream.Seek(currentItemIDDataEndPos, SeekOrigin.Begin);
        }
        return lastName;
    }

    private static string ReadNullTerminatedAnsiString(BinaryReader reader)
    {
        var sb = new StringBuilder();
        byte b;
        // Loop while not at end of stream and byte read is not null terminator
        while (reader.BaseStream.Position < reader.BaseStream.Length && (b = reader.ReadByte()) != 0)
        {
            sb.Append((char)b);
        }
        return sb.ToString();
    }

    private static string ReadNullTerminatedUnicodeString(BinaryReader reader)
    {
        var sb = new StringBuilder();
        // Loop while there are at least 2 bytes left to read for a char or null terminator
        while (reader.BaseStream.Position + 1 < reader.BaseStream.Length) 
        {
            byte b1 = reader.ReadByte();
            byte b2 = reader.ReadByte();
            if (b1 == 0 && b2 == 0) break; // Null terminator (U+0000)
            sb.Append((char)((b2 << 8) | b1)); // UTF-16 LE
        }
        return sb.ToString();
    }

    private static string ReadLengthPrefixedString(BinaryReader reader, bool isUnicode)
    {
        if (reader.BaseStream.Position + 2 > reader.BaseStream.Length) // Check for length field
             throw new FormatException("Invalid LNK: Unexpected EOF reading string length.");
        
        ushort charCount = reader.ReadUInt16();
        if (charCount == 0) return string.Empty;

        int bytesToRead = isUnicode ? charCount * 2 : charCount;
        if (reader.BaseStream.Position + bytesToRead > reader.BaseStream.Length) // Check for string data
            throw new FormatException("Invalid LNK: Unexpected EOF reading string data, or string length exceeds available data.");

        byte[] buffer = reader.ReadBytes(bytesToRead);
        
        return isUnicode ? Encoding.Unicode.GetString(buffer) : Encoding.Default.GetString(buffer);
    }
}