程序自动更新
> 系统所需：.net framework 4.0

> IIS部署：需要部署应用程序，并且根目录下必须有update.json文件（格式参照examples文件夹中的文件）

> 客户端更新网址配置：请修改AutoUpdate.exe.config文件中update_url的配置值

> 调用规则：AutoUpdate.exe 应用名（对应json文件中的product） 版本号

> 例如:AutoUpdate.exe myproduct 1.0.2

> 更新文件会自动保存在客户端根目录的download文件夹中
