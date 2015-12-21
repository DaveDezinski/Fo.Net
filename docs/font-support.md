# FO.NET
## Font Support

FO.NET includes arbitrary font support allowing XSL-FO developers to utilise any font present in the Windows Font directory. Individual fonts may be referenced in FO documents via their family name without the necessity for additional configuration and metrics generation.

FO.NET also allows you to control how the font should be handled in the generated PDF file. For more information please see the section [Font Linking, Embedding and Subsetting](./Section-4.1:-Font-Linking,-Embedding-and-Subsetting).

Additional information regarding fonts can be found in the sections [Private Fonts](./Section-4.2:-Private-Fonts), [Font Kerning](./Section-4.3:-Font-Kerning) and [Font Limitations](./Section-4.4:-Font-Limitations).

XSL-FO Font Example:

```
<?xml version="1.0" encoding="utf-8"?>
<fo:root xmlns:fo="http://www.w3.org/1999/XSL/Format">

  <!-- layout-master-set removed intentionally for readability -->

  <fo:page-sequence master-reference="simple">
    <fo:flow flow-name="xsl-region-body">

      <!-- Regular -->
      <fo:block font-size="18pt" font-family="Verdana">
        This is Verdana Regular
      </fo:block>

      <!-- Bold -->
      <fo:block font-size="18pt" font-family="Verdana" font-weight="bold">
        This is Verdana Bold
      </fo:block>

      <!-- Italic -->
      <fo:block font-size="18pt" font-family="Verdana" font-style="italic">
        This is Verdana Italic.
      </fo:block>

    </fo:flow>
  </fo:page-sequence>

</fo:root>
```


## Font Linking, Embedding and Subsetting

FO.NET supports three different types of font embedding: Link, Embed and Subset. This section will describe each embedding policy, the advantages and disadvantages of each and finish with a comparison matrix which will help you decide which policy best suits your requirements.

### Linking
The font program is referenced by name in the rendered PDF. Anyone who views a rendered PDF with a linked font program must have that font installed on their computer otherwise it will not display correctly. 

### Embedding
The entire font program is embedded in the rendered PDF. Embedding the entire font program guarantees the PDF will display as intended by the author on all computers, however this method does possess several disadvantages:

Font programs can be extremely large and will significantly increase the size of the rendered PDF. For example, the MS Gothic TrueType collection is 8MB!

Certain font programs cannot be embedded due to license restrictions. If you attempt to embed a font program that disallows embedding, FO.NET will substitute the font with a base 14 font and generate a warning message.

### Subseting
Subsetting a font will generate a new font that is embedded in the rendered PDF that contains only the glyphs referenced by the FO document. For example, if a particular FO document utilised the Verdana font referencing only the character 'A', a subsetted font would be generated at run-time containing only the information necessary to render the character 'A'. 

Subsetting provides the benefits of embedding and significantly reduces the size of the font program. However, small processing overhead is incurred to generated the subsetted font.

```
void Example()
{
    FonetDriver driver = FonetDriver.Make();
    driver.Renderer = RendererEngine.PDF;

    // Font embedding/linking is set via PdfRendererOptions class
    PdfRendererOptions options = new PdfRendererOptions();

    // Use FontType enumeration to specify either Link, Embed or Subset
    options.FontType = FontType.Link;
    //options.FontType = FontType.Embed;
    //options.FontType = FontType.Subset;

    driver.Options = options;
}
```

### What Font Embedding Policy Should I Use?

This section will provides a comparison matrix to help you decide which embedding policy best suits your requirements:

| # | Link | Embed | Subset |
|---|------|-------|--------|
| Size | Smallest - font is not embedded | Potentially very large since entire font is embedded in PDF | Significantly more efficient than embedding. Size increases with each character referenced in FO document. |
| Performance | Best | Minor overhead incurred to read and embed font data | Minor overhead incurred to read, subset and embed font data |
| Any Unicode character | Restricted to code page 1252 | Any character | Any character |
| PDF Integrity | Not guaranteed since PDF utilises fonts on host machine | Guaranteed | Guaranteed |
| Synthesising | Yes | No | No |


## Private Fonts

In the vast majority of cases no additional programming effort is required to utilise a TrueType/OpenType font. However, there may be instances where a font is not or cannot be installed in the Windows Fonts directory. FO.NET is still capable of using this font as a private font.

Private fonts are added via the `PdfRendererOptions` class as shown below.

```
void Example()
{
    // Add private font not installed in Fonts directoryPdfRendererOptions options = new PdfRendererOptions();
    options.AddPrivateFont(new FileInfo(@"\\server\fonts\specialfont.otf"));

    // The driver defaults to a PDF renderer
    FonetDriver driver = FonetDriver.Make();
    driver.Options = options;
    driver.Render("hello.xml", "hello.pdf");
}
```

## Font Kerning

Kerning permits FO.NET to adjust the inter-character spacing between pairs of characters depending on context to make them more visually appealing. Note that by default kerning is disabled since it adds a minor processing overhead.

### Enabling Kerning via Command Line

To enable kerning via the command line tool, specify the "-kerning" switch. For example:

```
fonet -kerning -fo document.fo -pdf document.pdf 
```

### Enabling Kerning in Code

The following code snippet illustrates enabling kerning support using the `PdfRendererOptions` class.

```
void Example()
{
    FonetDriver driver = FonetDriver.Make();
    driver.Renderer = RendererEngine.PDF;

    // Enable kerning by setting the Kerning property to true
    PdfRendererOptions options = new PdfRendererOptions();
    options.Kerning = true;

    driver.Options = options;
}
```

## Font Limitations

### Font Linking

Any font that is linked in the rendered PDF is restricted to Windows code page 1252, often called the "Windows ANSI" encoding. Please visit the following link to view a listing of Windows Code Page 1252. 

When an attempt is made to use a variant of a font that does not exist in the Windows Font directory, FO.NET will instruct Adobe Acrobat to synthesise the font. For example, if an FO document uses "SuperFont Bold" but only a regular variant is available, Acrobat will attempt to synthesise a bold font when it renders the PDF. Synthesising relies on the operating system making concerted "guesses" regarding the layout of each glyph and, therefore, will not always produce acceptable results. Wherever possible, use only fonts that are fully installed.

### Font Embedding
FO.NET does not support font synthesising when embedding fonts. 

### Font Subsetting
FO.NET does not support font synthesising when subsetting fonts.

Editing a PDF with a subsetted font may not be possible since it contains only a selection of the overall number of glyphs. Merging two PDFs that contain a different subset of the same font can lead to missing characters.
