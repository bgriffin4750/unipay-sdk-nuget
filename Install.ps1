param($installPath, $toolsPath, $package, $project)

$fileName = "UniPay_SDK.dll";
$file = $project.ProjectItems.Item($fileName);

If ($file -eq $null)
{
    $project.ProjectItems.AddFromFile($fileName);
    $file = $project.ProjectItems.Item($fileName);
}

$file.Properties.Item("CopyToOutputDirectory").Value = [int]1;
