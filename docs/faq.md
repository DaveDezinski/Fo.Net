# FO.NET
## FAQ

## Does FO.NET support SVG?
Unfortunately FO.NET does not support SVG. FO.NET's support for image formats includes WMF/EMF metafiles, which are both vector formats. Also refer to 'What image formats does FO.NET support?' below.

You could use the excellent [SVG# project](http://sourceforge.net/projects/svgdomcsharp) to render an SVG document to disk/memory and include the image in an FO document to be rendered by FO.NET.

## Is FO.NET a port of Apache FOP to .NET?
Yes. It is based on version 0.20.4 of FOP and does not follow FOP's subsequent progress.

FO.NET consists of 100% C# managed code. It is not just .NET integration, but a full port of FOP to the .NET environment.

## Does FO.NET have arbitrary font support?
FO.NET supports all TrueType and TrueType flavoured OpenType fonts. It can therefore render text in any language supported by the chosen font (though only left-to-right, top-to-bottom writing mode is supported).

## Does FO.NET support other output formats?
FO.NET only supports PDF output.

## Does FO.NET support Unicode? For example, can I use special characters like ohmega?
FO.NET can handle any code point supported by Unicode and by the current font in use.

## What image formats does FO.NET support?
Internally FO.NET uses the .NET System.Drawing namespace to handle external graphics. This means that FO.NET supports all the formats supported by .NET.

## Are there performance issues involved with using images?
Where possible, we recommend using JPEG images as FO.NET is optimised to read these image files.

## Can I use a barcode font with FO.NET?
FO.NET supports all TrueType and TrueType flavoured OpenType fonts. It can render barcodes simply by referencing one of the many available barcode fonts.

## Is FO.NET suitable for handling large amounts of data?
How well FO.NET handles large amounts of data depends on the complexity of the XSL-FO. For reasonably complex XSL-FO we have seen processing times of about 1 second per page.

## Is there a way to speed up the processing time of a large job?
Avoiding long running page sequences is the most effective method used to speed up processing time. Each new page sequence should be started as soon as possible. Many reports have natural page breaks, such as the chapters, records or invoices.

## Is FO.NET in any way Java-based; are there any external dependencies bar .NET?
No.