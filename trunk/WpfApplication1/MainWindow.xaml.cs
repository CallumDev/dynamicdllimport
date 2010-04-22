using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Security;
using System.Reflection;
using System.IO;

using RaisingStudio;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            dynamic user32 = new DynamicDllImport("user32.dll", callingConvention : CallingConvention.Winapi);
            user32.MessageBox(0, "Hello World", "Platform Invoke Sample", 0);

            dynamic asmproject = new DynamicDllImport("asmproject.dll");
            asmproject.nothing();

            for (int i = 0; i < 20; i++)
            {
                int value = asmproject.add<int>(3, 4);
                Console.WriteLine(value);             
            }
 
            string s = asmproject.hello<string>("h");
            Console.WriteLine(s);
        }

        internal class Sdl
        {
            public const int SDL_HWSURFACE = 1;
            public const int SDL_DOUBLEBUF = 1073741824;
            public const int SDL_ANYFORMAT = 268435456;

            public const int SDL_INIT_EVERYTHING = 65535;

            public const int SDL_QUIT = 12;

            [StructLayout(LayoutKind.Sequential, Pack = 4)]
            public struct SDL_Rect
            {
                public short x;
                public short y;
                public short w;
                public short h;

                public SDL_Rect(short x, short y, short w, short h)
                {
                    this.x = x;
                    this.y = y;
                    this.w = w;
                    this.h = h;
                }
            }

            [StructLayout(LayoutKind.Explicit)]
            public struct SDL_Event
            {
                [FieldOffset(0)]
                public byte type;

                [FieldOffset(0)]
                public SDL_QuitEvent quit;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 4)]
            public struct SDL_QuitEvent
            {
                public byte type;
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            dynamic sdl = new DynamicDllImport("SDL.dll", CharSet.Ansi);
            dynamic sdlImage = new DynamicDllImport("SDL_image.dll", CharSet.Ansi);

            int flags = (Sdl.SDL_HWSURFACE | Sdl.SDL_DOUBLEBUF | Sdl.SDL_ANYFORMAT);
            int width = 800;
            int height = 480;
            int bpp = 16;

            try
            {
                sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
                sdl.SDL_WM_SetCaption("WpfApplication1", "");

                IntPtr surfacePtr = sdl.SDL_SetVideoMode<IntPtr>(
                    width,
                    height,
                    bpp,
                    flags);

                int rmask = 0x00000000;
                int gmask = 0x00ff0000;
                int bmask = 0x0000ff00;
                int amask = 0x000000ff;
                IntPtr rgbSurfacePtr = sdl.SDL_CreateRGBSurface<IntPtr>(
                    flags,
                    width,
                    height,
                    bpp,
                    rmask,
                    gmask,
                    bmask,
                    amask);

                Sdl.SDL_Rect rect = new Sdl.SDL_Rect(
                    0,
                    0,
                    (short)width,
                    (short)height);
                int result = sdl.SDL_FillRect<int>(rgbSurfacePtr, ref rect, 0);

                string tempFileName = System.IO.Path.GetTempFileName();
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WpfApplication1.desktop.png");
                var writeStream = System.IO.File.Create(tempFileName);
                var readStream = new BinaryReader(stream);
                using (var binaryWriter = new BinaryWriter(writeStream))
                {
                    binaryWriter.Write(readStream.ReadBytes((int)readStream.BaseStream.Length));
                    binaryWriter.Flush();
                    binaryWriter.Close();
                }

                IntPtr image = sdlImage.IMG_Load<IntPtr>(tempFileName);

                Sdl.SDL_Rect srcrect = new Sdl.SDL_Rect();
                srcrect.x = 0;
                srcrect.y = 0;
                srcrect.w = (short)width;
                srcrect.h = (short)height;
                Sdl.SDL_Rect dstrect = srcrect;
                sdl.SDL_UpperBlit<int>(image, ref srcrect, surfacePtr, ref dstrect);

                sdl.SDL_UpdateRect(surfacePtr, 0, 0, width, height);

                Sdl.SDL_Event evt;
                while (sdl.SDL_WaitEvent(out evt) != 0)
                {
                    if (evt.type == Sdl.SDL_QUIT)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                sdl.SDL_Quit();
                throw;
            }
            finally
            {
                sdl.SDL_Quit();
            }
        }
    }
}
