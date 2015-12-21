# FO.NET
## Configuring PDF Options

FO.NET gives you control over many aspects of the generated PDF documents: 
* Passwords that control access to generated PDF documents.
* The Author, Title and Subject document properties.
* The keywords embedded in the document properties.
* Font embedding policy.
* Addition of private fonts.
* Whether or not to enable kerning.

PDF options are specified by passing an instance of `PdfRendererOptions` to the `FonetDriver`.

```
Imports Fonet
Imports Fonet.Render.Pdf

Module FonetTest

    Sub Main()
        Dim options As New PdfRendererOptions()
        options.Author = "David Brent"
        options.Title = "How to Motivate Your Staff"
        options.Subject = "Management Guide"
        options.EnableModify = False
        options.EnableAdd = False
        options.EnableCopy = False
        options.EnablePrinting = True
        options.OwnerPassword = "slough"

        Dim driver As FonetDriver = FonetDriver.Make
        driver.Options = options
        driver.Render("input.fo", "output.pdf")
    End Sub

End Module
```
