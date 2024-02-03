using System;
using System.IO;
using System.Runtime.InteropServices;



public class brotli
{

#if !UNITY_WEBPLAYER || UNITY_EDITOR


#if UNITY_5_4_OR_NEWER
#if (UNITY_ANDROID || UNITY_STANDALONE_LINUX) && !UNITY_EDITOR || UNITY_EDITOR_LINUX
		private const string libname = "brotli";
#elif UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
    private const string libname = "libbrotli";
#endif
#else
#if (UNITY_ANDROID || UNITY_STANDALONE_LINUX) && !UNITY_EDITOR
		private const string libname = "brotli";
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		private const string libname = "libbrotli";
#endif
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX
#if (UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX) && !UNITY_EDITOR_WIN
			[DllImport(libname, EntryPoint = "setPermissions")]
			internal static extern int setPermissions(string filePath, string _user, string _group, string _other);
#endif

    [DllImport(libname, EntryPoint = "brCompress"
#if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		, CallingConvention = CallingConvention.Cdecl
#endif
    )]
    internal static extern int brCompress(string inFile, string outFile, IntPtr proc, int quality, int lgwin, int lgblock, int mode);

    [DllImport(libname, EntryPoint = "brDecompresss"
#if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		, CallingConvention = CallingConvention.Cdecl
#endif
    )]
    internal static extern int brDecompresss(string inFile, string outFile, IntPtr proc, IntPtr FileBuffer, int fileBufferLength);

    [DllImport(libname, EntryPoint = "brReleaseBuffer"
#if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		, CallingConvention = CallingConvention.Cdecl
#endif
    )]
    internal static extern int brReleaseBuffer(IntPtr buffer);

    [DllImport(libname, EntryPoint = "brCompressBuffer"
#if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		, CallingConvention = CallingConvention.Cdecl
#endif
    )]
    internal static extern IntPtr brCompressBuffer(int bufferLength, IntPtr buffer, IntPtr encodedSize, IntPtr proc, int quality, int lgwin, int lgblock, int mode);

    //this will work on small files with one meta block
    [DllImport(libname, EntryPoint = "brGetDecodedSize"
#if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		, CallingConvention = CallingConvention.Cdecl
#endif
    )]
    internal static extern int brGetDecodedSize(int bufferLength, IntPtr buffer);

    [DllImport(libname, EntryPoint = "brDecompressBuffer"
#if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		, CallingConvention = CallingConvention.Cdecl
#endif
    )]
    internal static extern int brDecompressBuffer(int bufferLength, IntPtr buffer, int outLength, IntPtr outbuffer);
#endif

#if (UNITY_IOS || UNITY_TVOS || UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
#if !UNITY_TVOS && !UNITY_WEBGL
		[DllImport("__Internal")]
		internal static extern int setPermissions(string filePath, string _user, string _group, string _other);
		[DllImport("__Internal")]
		internal static extern int brCompress( string inFile, string outFile, IntPtr proc, int quality, int lgwin, int lgblock, int mode);
        [DllImport("__Internal")]
		internal static extern int brDecompresss(string inFile, string outFile, IntPtr proc, IntPtr FileBuffer, int fileBufferLength);
        
#endif
        [DllImport("__Internal")]
		internal static extern int brReleaseBuffer(IntPtr buffer);
#if !UNITY_WEBGL
        [DllImport("__Internal")]
        internal static extern IntPtr brCompressBuffer( int bufferLength, IntPtr buffer, IntPtr encodedSize, IntPtr proc, int quality, int lgwin, int lgblock, int mode);
#endif
		//this will work on small files with one meta block
		[DllImport("__Internal")]
		internal static extern int brGetDecodedSize(int bufferLength, IntPtr buffer);
        [DllImport("__Internal")]
        internal static extern int brDecompressBuffer(int bufferLength, IntPtr buffer,  int outLength, IntPtr outbuffer);
#endif

#if (!UNITY_TVOS && !UNITY_WEBGL) || UNITY_EDITOR
    // set permissions of a file in user, group, other.
    // Each string should contain any or all chars of "rwx".
    // returns 0 on success
    public static int setFilePermissions(string filePath, string _user, string _group, string _other)
    {
#if (UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX || UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR_WIN
            if(!File.Exists(filePath)) return -1;
			return setPermissions(filePath, _user, _group, _other);
#else
        return -1;
#endif
    }

    public static int compressFile(string inFile, string outFile, ulong[] proc, int quality = 9, int lgwin = 19, int lgblock = 0, int mode = 0)
    {
        if (!File.Exists(inFile)) return -5;
        if (quality < 0) quality = 1; if (quality > 11) quality = 11;
        if (lgwin < 10) lgwin = 10; if (lgwin > 24) lgwin = 24;
        GCHandle cbuf = GCHandle.Alloc(proc, GCHandleType.Pinned);
        int res = brCompress(@inFile, @outFile, cbuf.AddrOfPinnedObject(), quality, lgwin, lgblock, mode);
        cbuf.Free();
        return res;
    }

    public static int decompressFile(string inFile, string outFile, ulong[] proc, byte[] FileBuffer = null)
    {
        if (FileBuffer == null && !File.Exists(inFile)) return -5;
        GCHandle cbuf = GCHandle.Alloc(proc, GCHandleType.Pinned);
        int res;
#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
			if(FileBuffer!= null) {
				GCHandle fbuf = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
				res = brDecompresss(null, @outFile, cbuf.AddrOfPinnedObject(), fbuf.AddrOfPinnedObject(), FileBuffer.Length);
				fbuf.Free();
				return res;
			}
#endif
        res = brDecompresss(@inFile, @outFile, cbuf.AddrOfPinnedObject(), IntPtr.Zero, 0);
        cbuf.Free();
        return res;
    }

#endif


    public static int getDecodedSize(byte[] inBuffer)
    {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        int res = brGetDecodedSize(inBuffer.Length, cbuf.AddrOfPinnedObject());
        cbuf.Free();
        return res;
    }

#if !UNITY_WEBGL || UNITY_EDITOR

    public static bool compressBuffer(byte[] inBuffer, ref byte[] outBuffer, ulong[] proc, bool includeSize = false, int quality = 9, int lgwin = 19, int lgblock = 0, int mode = 0)
    {
        if (quality < 0) quality = 1; if (quality > 11) quality = 11;
        if (lgwin < 10) lgwin = 10; if (lgwin > 24) lgwin = 24;

        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int size = 0;
        byte[] bsiz = null;
        int[] esiz = new int[1];//the compressed size
        GCHandle ebuf = GCHandle.Alloc(esiz, GCHandleType.Pinned);

        if (includeSize)
        {
            bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!BitConverter.IsLittleEndian) Array.Reverse(bsiz);
        }

        if (proc == null) proc = new ulong[1];
        GCHandle pbuf = GCHandle.Alloc(proc, GCHandleType.Pinned);

        ptr = brCompressBuffer(inBuffer.Length, cbuf.AddrOfPinnedObject(), ebuf.AddrOfPinnedObject(), pbuf.AddrOfPinnedObject(), quality, lgwin, lgblock, mode);

        cbuf.Free(); ebuf.Free(); pbuf.Free();

        if (ptr == IntPtr.Zero) { brReleaseBuffer(ptr); esiz = null; bsiz = null; return false; }

        System.Array.Resize(ref outBuffer, esiz[0] + size);

        //add the uncompressed size to the buffer
        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[i + esiz[0]] = bsiz[i]; }

        Marshal.Copy(ptr, outBuffer, 0, esiz[0]);

        brReleaseBuffer(ptr);
        esiz = null;
        bsiz = null;

        return true;
    }

    public static byte[] compressBuffer(byte[] inBuffer, int[] proc, bool includeSize = false, int quality = 9, int lgwin = 19, int lgblock = 0, int mode = 0)
    {
        if (quality < 0) quality = 1; if (quality > 11) quality = 11;
        if (lgwin < 10) lgwin = 10; if (lgwin > 24) lgwin = 24;

        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int size = 0;
        byte[] bsiz = null;
        int[] esiz = new int[1];//the compressed size
        GCHandle ebuf = GCHandle.Alloc(esiz, GCHandleType.Pinned);

        if (includeSize)
        {
            bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!BitConverter.IsLittleEndian) Array.Reverse(bsiz);
        }

        if (proc == null) proc = new int[1];
        GCHandle pbuf = GCHandle.Alloc(proc, GCHandleType.Pinned);

        ptr = brCompressBuffer(inBuffer.Length, cbuf.AddrOfPinnedObject(), ebuf.AddrOfPinnedObject(), pbuf.AddrOfPinnedObject(), quality, lgwin, lgblock, mode);

        cbuf.Free(); ebuf.Free(); pbuf.Free();

        if (ptr == IntPtr.Zero) { brReleaseBuffer(ptr); esiz = null; bsiz = null; return null; }

        byte[] outBuffer = new byte[esiz[0] + size];

        //add the uncompressed size to the buffer
        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[i + esiz[0]] = bsiz[i]; }

        Marshal.Copy(ptr, outBuffer, 0, esiz[0]);

        brReleaseBuffer(ptr);
        esiz = null;
        bsiz = null;

        return outBuffer;
    }

    public static int compressBuffer(byte[] inBuffer, byte[] outBuffer, int[] proc, bool includeSize = false, int quality = 9, int lgwin = 19, int lgblock = 0, int mode = 0)
    {
        if (quality < 0) quality = 1; if (quality > 11) quality = 11;
        if (lgwin < 10) lgwin = 10; if (lgwin > 24) lgwin = 24;

        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int size = 0;
        byte[] bsiz = null;
        int res = 0;
        int[] esiz = new int[1];//the compressed size
        GCHandle ebuf = GCHandle.Alloc(esiz, GCHandleType.Pinned);

        // if the uncompressed size of the buffer should be included. This is a hack since brotli lib does not support this on larger buffers.
        if (includeSize)
        {
            bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!BitConverter.IsLittleEndian) Array.Reverse(bsiz);
        }

        if (proc == null) proc = new int[1];
        GCHandle pbuf = GCHandle.Alloc(proc, GCHandleType.Pinned);

        ptr = brCompressBuffer(inBuffer.Length, cbuf.AddrOfPinnedObject(), ebuf.AddrOfPinnedObject(), pbuf.AddrOfPinnedObject(), quality, lgwin, lgblock, mode);

        cbuf.Free(); ebuf.Free(); pbuf.Free();
        res = esiz[0];

        if (ptr == IntPtr.Zero || outBuffer.Length < (esiz[0] + size)) { brReleaseBuffer(ptr); esiz = null; bsiz = null; return 0; }

        Marshal.Copy(ptr, outBuffer, 0, esiz[0]);

        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[i + esiz[0]] = bsiz[i]; }

        brReleaseBuffer(ptr);
        esiz = null;
        bsiz = null;

        return res + size;
    }

#endif

    public static bool decompressBuffer(byte[] inBuffer, ref byte[] outBuffer, bool useFooter = false, int unCompressedSize = 0)
    {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        int uncompressedSize = 0, res2 = inBuffer.Length;

        if (unCompressedSize == 0)
        {
            if (useFooter)
            {
                res2 -= 4;
                uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
            }
            else
            {
                //use the brotli native method to get the uncompressed size (this will work on buffers with one metablock)
                uncompressedSize = getDecodedSize(inBuffer);
            }
        }
        else
        {
            uncompressedSize = unCompressedSize;
        }

        System.Array.Resize(ref outBuffer, uncompressedSize);

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

        int res = brDecompressBuffer(inBuffer.Length, cbuf.AddrOfPinnedObject(), uncompressedSize, obuf.AddrOfPinnedObject());

        cbuf.Free();
        obuf.Free();

        if (res == 1) return true; else return false;
    }


    public static byte[] decompressBuffer(byte[] inBuffer, bool useFooter = false, int unCompressedSize = 0)
    {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        int uncompressedSize = 0, res2 = inBuffer.Length;

        if (unCompressedSize == 0)
        {
            if (useFooter)
            {
                res2 -= 4;
                uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
            }
            else
            {
                //use the brotli native method to get the uncompressed size (this will work on buffers with one metablock)
                uncompressedSize = getDecodedSize(inBuffer);
            }
        }
        else
        {
            uncompressedSize = unCompressedSize;
        }

        byte[] outBuffer = new byte[uncompressedSize];

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

        int res = brDecompressBuffer(inBuffer.Length, cbuf.AddrOfPinnedObject(), uncompressedSize, obuf.AddrOfPinnedObject());

        cbuf.Free();
        obuf.Free();

        if (res == 1) return outBuffer; else return null;
    }


    public static int decompressBuffer(byte[] inBuffer, byte[] outBuffer, bool useFooter = false, int unCompressedSize = 0)
    {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        int uncompressedSize = 0, res2 = inBuffer.Length;

        if (unCompressedSize == 0)
        {
            if (useFooter)
            {
                res2 -= 4;
                uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
            }
            else
            {
                //use the brotli native method to get the uncompressed size (this will work on buffers with one metablock)
                uncompressedSize = getDecodedSize(inBuffer);
            }
        }
        else
        {
            uncompressedSize = unCompressedSize;
        }

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

        int res = brDecompressBuffer(inBuffer.Length, cbuf.AddrOfPinnedObject(), uncompressedSize, obuf.AddrOfPinnedObject());

        cbuf.Free();
        obuf.Free();

        if (res == 1) return uncompressedSize; else return 0;
    }


#endif
}

