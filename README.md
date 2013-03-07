# PathFind

_A Windows app to demonstrate path-finding algorithms._

## Project Setup

1. Download and install [Visual C# Express 2010](http://www.microsoft.com/visualstudio/eng/products/visual-studio-2010-express).
2. Run `GetThirdPartyLibraries.bat` to download required third-party libraries.

## Testing

1. Build the PathFindTests project.
2. Run NUnit, one of the required libraries, and open `Tests\bin\Debug\PathFindTests.dll`.
3. From NUnit's `Tools` menu,
    * Choose `Settings...`
    * Go to the `Assembly Reload` section
    * Enable `Reload when test assembly changes` and `Re-run last tests run`
    * Tests will run automatically whenever `PathFindTests.dll` builds