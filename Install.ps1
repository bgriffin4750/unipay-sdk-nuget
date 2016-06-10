param($installPath, $toolsPath, $package, $project)

$file = $project.ProjectItems.Item("libtidy.dll");

If ($file -eq $null)
{
    $project.ProjectItems.AddFromFile("libtidy.dll");
    $file = $project.ProjectItems.Item("libtidy.dll");
}

$file.Properties.Item("CopyToOutputDirectory").Value = [int]1;
