# FO.NET
## Hello World! Example

As most programmers will testify, it is customary to begin with a "Hello, World!" example. Here is the XSL-FO document that FO.NET will render as a PDF:

**XSL-FO File (`hello.fo`):**

```
<?xml version="1.0" encoding="utf-8"?>
<fo:root xmlns:fo="http://www.w3.org/1999/XSL/Format">

  <fo:layout-master-set>
    <fo:simple-page-master master-name="simple"
                  page-height="29.7cm"
                  page-width="21cm"
                  margin-top="1cm"
                  margin-bottom="2cm"
                  margin-left="2.5cm"
                  margin-right="2.5cm">
      <fo:region-body margin-top="3cm"/>
      <fo:region-before extent="3cm"/>
      <fo:region-after extent="1.5cm"/>
    </fo:simple-page-master>
  </fo:layout-master-set>

  <fo:page-sequence master-reference="simple">
    <fo:flow flow-name="xsl-region-body">
      <fo:block font-size="18pt" color="black" text-align="center">
        Hello, World!
      </fo:block>
    </fo:flow>
  </fo:page-sequence>

</fo:root>
```

The following code can be used to convert the above XSL-FO document to a PDF file:

**Hello World C# Example**

```
using System.IO;
using Fonet;

namespace FonetExample {
    class HelloWorld {
        static void Main(string[] args) {
            FonetDriver driver = FonetDriver.Make();
            driver.Render("hello.fo", "hello.pdf");
        }
    }
}
```

The above class will create a file called `hello.pdf` from the file `hello.fo` located in the current working directory.
