param($installPath, $toolsPath, $package, $project)

# open json.net splash page on package install
# don't open if json.net is installed as a dependency

try
{
  $url = "http://james.newtonking.com/json"
  $dte2 = Get-Interface $dte ([EnvDTE80.DTE2])

  if ($dte2.ActiveWindow.Caption -eq "Package Manager Console")
  {
    # user is installing from VS NuGet console
    # get reference to the window, the console host and the input history
    # show webpage if "install-package newtonsoft.json" was last input

    $consoleWindow = $(Get-VSComponentModel).GetServic