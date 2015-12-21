# FO.NET
## FonetDriver Class

An instance of `Fonet.FonetDriver` is obtained by calling the static factory method `FonetDriver.Make`.

```
using Fonet;

namespace FonetExample
{
    class HelloWorld
    {
        static void Main(string[] args)
        {
            FonetDriver driver = FonetDriver.Make();
            // do something with driver
        }
    }
}
```

```
Imports Fonet

Module FonetTest
    Sub Main()
        Dim driver As FonetDriver = FonetDriver.Make
        ' do something with driver 
    End Sub
End Module
```

Once an instance of `FonetDriver` has been obtained, PDF documents can easily be created by calling the Render method. The Render method is passed two arguments:
* The source XSL-FO document that describes what to render.
* The destination stream or file that the PDF document should be written to.

```
driver.Render("input.fo", "output.pdf");
```

A variety of overloaded forms of `Render` are available.
