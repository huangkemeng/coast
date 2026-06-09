$name = $args[0];
[bool]$name
if ($name)
{
    dotnet ef migrations add $name --project ../Coast.Api.Infrastructure
}
else
{
    Write-Host "请输出本次迁移的名称！"
}