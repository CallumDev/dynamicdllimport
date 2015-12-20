use "dynamic" to make PInvoke simple.

```
dynamic user32 = new DynamicDllImport("user32.dll", callingConvention : CallingConvention.Winapi);
user32.MessageBox(0, "Hello World", "Platform Invoke Sample", 0);
```

```
dynamic asmproject = new DynamicDllImport("asmproject.dll");
int value = asmproject.add<int>(3, 4);
Console.WriteLine(value);  
```

```
dynamic sdl = new DynamicDllImport("SDL.dll", CharSet.Ansi);
Sdl.SDL_Rect rect = new Sdl.SDL_Rect(
                    0,
                    0,
                    (short)width,
                    (short)height);
int result = sdl.SDL_FillRect<int>(rgbSurfacePtr, ref rect, 0);
```

```
Sdl.SDL_Event evt;
while (sdl.SDL_WaitEvent(out evt) != 0)
{
   if (evt.type == Sdl.SDL_QUIT)
   {
       break;
    }
}
```