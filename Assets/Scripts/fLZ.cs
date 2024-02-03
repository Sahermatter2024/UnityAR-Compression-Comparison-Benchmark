using System;
using System.Runtime.InteropServices;



public class fLZ
{

#if !UNITY_WEBPLAYER || UNITY_EDITOR

    internal static bool isle = BitConverter.IsLittleEndian;

#if UNITY_5_4_OR_NEWER
#if (UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_WEBGL) && !UNITY_EDITOR || UNITY_EDITOR_LINUX
		private const string libname = "fastlz";
#elif UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
    private const string libname = "libfastlz";
#endif
#else
#if (UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_WEBGL) && !UNITY_EDITOR
		private const string libname = "fastlz";
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		private const string libname = "libfastlz";
#endif
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX

#if (!UNITY_WEBGL || UNITY_EDITOR)
#if (UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX) && !UNITY_EDITOR_WIN
				[DllImport(libname, EntryPoint = "fsetPermissions")]
				internal static extern int fsetPermissions(string filePath, string _user, string _group, string _other);
#endif
    [DllImport(libname, EntryPoint = "fLZcompressFile"
#if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		        , CallingConvention = CallingConvention.Cdecl
#endif
    )]
    internal static extern int fLZcompressFile(int level, string inFile, string outFile, bool overwrite, IntPtr percent);

    [DllImport(libname, EntryPoint = "fLZdecompressFile"
#if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		        , CallingConvention = CallingConvention.Cdecl
#endif
    )]
    internal static extern int fLZdecompressFile(string inFile, string outFile, bool overwrite, IntPtr percent, IntPtr FileBuffer, int fileBufferLength);
#endif

    [DllImport(libname, EntryPoint = "fLZreleaseBuffer"
#if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		    , CallingConvention = CallingConvention.Cdecl
#endif
    )]
    internal static extern int fLZreleaseBuffer(IntPtr buffer);

    [DllImport(libname, EntryPoint = "fLZcompressBuffer"
#if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		    , CallingConvention = CallingConvention.Cdecl
#endif
    )]
    internal static extern IntPtr fLZcompressBuffer(IntPtr buffer, int bufferLength, int level, ref int v);

    [DllImport(libname, EntryPoint = "fLZdecompressBuffer"
#if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		    , CallingConvention = CallingConvention.Cdecl
#endif
    )]
    internal static extern int fLZdecompressBuffer(IntPtr buffer, int bufferLength, IntPtr outbuffer);
#endif

#if (UNITY_IOS || UNITY_TVOS || UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
#if (UNITY_IOS || UNITY_IPHONE) && !UNITY_WEBGL
			[DllImport("__Internal")]
			internal static extern int fsetPermissions(string filePath, string _user, string _group, string _other);
			[DllImport("__Internal")]
			internal static extern int fLZcompressFile(int level, string inFile, string outFile, bool overwrite, IntPtr percent);
			[DllImport("__Internal")]
			internal static extern int fLZdecompressFile(string inFile, string outFile, bool overwrite, IntPtr percent, IntPtr FileBuffer, int fileBufferLength);
#endif
#if (UNITY_IPHONE || UNITY_IOS || UNITY_TVOS || UNITY_WEBGL)
			[DllImport("__Internal")]
			internal static extern int fLZreleaseBuffer(IntPtr buffer);
			[DllImport("__Internal")]
			internal static extern IntPtr fLZcompressBuffer(IntPtr buffer, int bufferLength, int level, ref int v);
			[DllImport("__Internal")]
			internal static extern int fLZdecompressBuffer(IntPtr buffer, int bufferLength, IntPtr outbuffer);
#endif
#endif

#if (!UNITY_WEBGL && !UNITY_TVOS) || UNITY_EDITOR
    // set permissions of a file in user, group, other.
    // Each string should contain any or all chars of "rwx".
    // returns 0 on success
    public static int setFilePermissions(string filePath, string _user, string _group, string _other)
    {
#if (UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX || UNITY_IOS || UNITY_TVOS || UNITY_IPHONE) && !UNITY_EDITOR_WIN
			return fsetPermissions(filePath, _user, _group, _other);
#else
        return -1;
#endif
    }


    //Compress a file to fLZ.
    //
    //Full paths to the files should be provided.
    //level:    level of compression (1 = faster/bigger, 2 = slower/smaller).
    //returns:  size of resulting archive in bytes
    //progress: provide a single item ulong array to get the progress of the compression in real time. (only when called from a thread/task)
    //

    public static int compressFile(string inFile, string outFile, int level, bool overwrite, ulong[] progress)
    {
        if (level < 1) level = 1;
        if (level > 2) level = 2;
        GCHandle ibuf = GCHandle.Alloc(progress, GCHandleType.Pinned);
        int res = fLZcompressFile(level, @inFile, @outFile, overwrite, ibuf.AddrOfPinnedObject());
        ibuf.Free();
        return res;
    }

    //Decompress an fLZ file.
    //
    //Full paths to the files should be provided.
    //returns: 1 on success.
    //progress: provide a single item ulong array to get the progress of the decompression in real time. (only when called from a thread/task)
    //FileBuffer: A buffer that holds an flz file. When assigned the function will decompress from this buffer and will ignore the filePath. (Linux, iOS, Android, MacOSX)
    //
    public static int decompressFile(string inFile, string outFile, bool overwrite, ulong[] progress, byte[] FileBuffer = null)
    {
        int res = 0;
        GCHandle ibuf = GCHandle.Alloc(progress, GCHandleType.Pinned);
#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
			if(FileBuffer != null) {
                GCHandle fbuf = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
				res = fLZdecompressFile(null, @outFile, overwrite, ibuf.AddrOfPinnedObject(), fbuf.AddrOfPinnedObject(), FileBuffer.Length);
				fbuf.Free(); ibuf.Free();
				return res;
			}
#endif
        res = fLZdecompressFile(inFile, @outFile, overwrite, ibuf.AddrOfPinnedObject(), IntPtr.Zero, 0);
        ibuf.Free();
        return res;
    }

#endif

    //Compress a byte buffer in fLZ format.
    //
    //inBuffer:     the uncompressed buffer.
    //outBuffer:    a referenced buffer that will be resized to fit the fLZ compressed data.
    //includeSize:  include the uncompressed size of the buffer in the resulted compressed one because fLZ does not include this.
    //level:        level of compression (1 = faster/bigger, 2 = slower/smaller).
    //returns true on success
    //
    // !!  If the input is not compressible, the returned buffer might be larger than the input buffer and not valid !!
    //
    // The  minimum input buffer size is 16.
    // The output buffer can not be smaller than 66 bytes
    //
    public static bool compressBuffer(byte[] inBuffer, ref byte[] outBuffer, int level, bool includeSize = true)
    {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int res = 0, size = 0;
        byte[] bsiz = null;

        //if the uncompressed size of the buffer should be included. This is a hack since fLZ lib does not support this.
        if (includeSize)
        {
            bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!isle) Array.Reverse(bsiz);
        }

        if (level < 1) level = 1;
        if (level > 2) level = 2;

        ptr = fLZcompressBuffer(cbuf.AddrOfPinnedObject(), inBuffer.Length, level, ref res);

        cbuf.Free();

        if (res == 0 || ptr == IntPtr.Zero) { fLZreleaseBuffer(ptr); return false; }

        System.Array.Resize(ref outBuffer, res + size);

        //add the uncompressed size to the buffer
        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[i + res] = bsiz[i];  /*Debug.Log(BitConverter.ToInt32(bsiz, 0));*/ }

        Marshal.Copy(ptr, outBuffer, 0, res);

        fLZreleaseBuffer(ptr);
        bsiz = null;

        return true;
    }


    //Compress a byte buffer in fLZ format.
    //
    //inBuffer:     the uncompressed buffer.
    //outBuffer:    a referenced buffer that will be resized to fit the fLZ compressed data.
    //includeSize:  include the uncompressed size of the buffer in the resulted compressed one because fLZ does not include this.
    //level:        level of compression (1 = faster/bigger, 2 = slower/smaller).
    //returns: a new buffer with the compressed data.
    //
    // !!  If the input is not compressible, the returned buffer might be larger than the input buffer and not valid !!
    //
    // The  minimum input buffer size is 16.
    // The output buffer can not be smaller than 66 bytes
    //
    public static byte[] compressBuffer(byte[] inBuffer, int level, bool includeSize = true)
    {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int res = 0, size = 0;
        byte[] bsiz = null;

        //if the uncompressed size of the buffer should be included. This is a hack since fLZ lib does not support this.
        if (includeSize)
        {
            bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!isle) Array.Reverse(bsiz);
        }

        if (level < 1) level = 1;
        if (level > 2) level = 2;

        ptr = fLZcompressBuffer(cbuf.AddrOfPinnedObject(), inBuffer.Length, level, ref res);
        cbuf.Free();

        if (res == 0 || ptr == IntPtr.Zero) { fLZreleaseBuffer(ptr); return null; }

        byte[] outBuffer = new byte[res + size];

        //add the uncompressed size to the buffer
        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[i + res] = bsiz[i];  /*Debug.Log(BitConverter.ToInt32(bsiz, 0));*/ }

        Marshal.Copy(ptr, outBuffer, 0, res);

        fLZreleaseBuffer(ptr);
        bsiz = null;

        return outBuffer;
    }


    //Decompress an fLZ compressed buffer to a referenced buffer.
    //
    //inBuffer: the fLZ compressed buffer
    //outBuffer: a referenced buffer that will be resized to store the uncompressed data.
    //useFooter: if the input Buffer has the uncompressed size info.
    //customLength: provide the uncompressed size of the compressed buffer. Not needed if the usefooter is used!
    //returns true on success
    //
    public static bool decompressBuffer(byte[] inBuffer, ref byte[] outBuffer, bool useFooter = true, int customLength = 0)
    {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        int uncompressedSize = 0, res2 = inBuffer.Length, ex = 0;

        //if the hacked in fLZ footer will be used to extract the uncompressed size of the buffer. If the buffer does not have a footer 
        //provide the known uncompressed size through the customLength integer.
        if (useFooter)
        {
            res2 -= 4;
            uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
            if (inBuffer.Length > uncompressedSize) ex = inBuffer.Length - uncompressedSize;
        }
        else
        {
            uncompressedSize = customLength;
            if (inBuffer.Length > outBuffer.Length) ex = inBuffer.Length - outBuffer.Length;
        }

        System.Array.Resize(ref outBuffer, uncompressedSize);

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);


        int res = fLZdecompressBuffer(cbuf.AddrOfPinnedObject(), uncompressedSize + ex, obuf.AddrOfPinnedObject());

        cbuf.Free();
        obuf.Free();

        if (res == 0) return true;

        return false;
    }

    //Decompress an flz compressed buffer to a referenced fixed size buffer.
    //
    //inBuffer: the flz compressed buffer
    //outBuffer: a referenced fixed size buffer where the data will get decompressed
    //useFooter: if the input Buffer has the uncompressed size info.
    //customLength: provide the uncompressed size of the compressed buffer. Not needed if the useFooter is used!
    //returns uncompressedSize
    //
    public static int decompressBufferFixed(byte[] inBuffer, ref byte[] outBuffer, bool safe = true, bool useFooter = true, int customLength = 0)
    {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        int uncompressedSize = 0, res2 = inBuffer.Length, ex = 0;

        //if the hacked in LZ4 footer will be used to extract the uncompressed size of the buffer. If the buffer does not have a footer 
        //provide the known uncompressed size through the customLength integer.
        if (useFooter)
        {
            res2 -= 4;
            uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
            if (inBuffer.Length > uncompressedSize) ex = inBuffer.Length - uncompressedSize;
        }
        else
        {
            uncompressedSize = customLength;
            if (inBuffer.Length > outBuffer.Length) ex = inBuffer.Length - outBuffer.Length;
        }

        //Check if the uncompressed size is bigger then the size of the fixed buffer. Then:
        //1. write only the data that fits in it.
        //2. or return a negative number. 
        //It depends on if we set the safe flag to true or not.
        if (uncompressedSize > outBuffer.Length)
        {
            if (safe) return -101; else uncompressedSize = outBuffer.Length;
        }

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

        int res = fLZdecompressBuffer(cbuf.AddrOfPinnedObject(), uncompressedSize + ex, obuf.AddrOfPinnedObject());

        cbuf.Free();
        obuf.Free();

        if (safe) { if (res != 0) return -101; }

        return uncompressedSize;
    }


    //Decompress an fLZ compressed buffer to a new buffer.
    //
    //inBuffer: the fLZ compressed buffer
    //useFooter: if the input Buffer has the uncompressed size info.
    //customLength: provide the uncompressed size of the compressed buffer. Not needed if the usefooter is used!
    //returns: a new buffer with the uncompressed data.
    //
    public static byte[] decompressBuffer(byte[] inBuffer, bool useFooter = true, int customLength = 0)
    {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        int uncompressedSize = 0, res2 = inBuffer.Length, ex = 0;

        //if the hacked in fLZ footer will be used to extract the uncompressed size of the buffer. If the buffer does not have a footer 
        //provide the known uncompressed size through the customLength integer.
        if (useFooter)
        {
            res2 -= 4;
            uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
            if (inBuffer.Length > uncompressedSize) ex = inBuffer.Length - uncompressedSize;
        }
        else
        {
            uncompressedSize = customLength;
            if (inBuffer.Length > customLength) ex = inBuffer.Length - customLength;
        }

        byte[] outBuffer = new byte[uncompressedSize];

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);


        int res = fLZdecompressBuffer(cbuf.AddrOfPinnedObject(), uncompressedSize + ex, obuf.AddrOfPinnedObject());

        cbuf.Free();
        obuf.Free();

        if (res != 0) return null;

        return outBuffer;
    }

#endif
}

