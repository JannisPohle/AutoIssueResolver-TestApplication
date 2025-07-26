using (var fs = new FileStream(filePath, FileMode.Open))
{
var content = new byte[fs.Length];
var bytesRead = 0;
while (bytesRead < fs.Length)
{
bytesRead += await fs.ReadAsync(content, bytesRead, content.Length - bytesRead);
}

return Encoding.UTF8.GetString(content);
}