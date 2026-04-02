# Windows 部署配置指南

## 第一步：生成 SSH 密钥（在 Windows 上执行）

打开 **PowerShell** 或 **Git Bash**，执行以下命令：

```powershell
# 创建 .ssh 目录（如果不存在）
mkdir -p ~\.ssh

# 生成 SSH 密钥对
ssh-keygen -t ed25519 -C "github-actions" -f ~/.ssh/github_actions_deploy
```

按提示操作（可以直接回车，不需要设置密码短语）。

## 第二步：将公钥添加到服务器

在 PowerShell 或 Git Bash 中执行：

```powershell
# 查看公钥内容
cat ~/.ssh/github_actions_deploy.pub
```

复制输出的全部内容，然后通过 SSH 连接到服务器并添加：

```powershell
# 连接到服务器（会提示输入密码：llll8023）
ssh root@69.5.23.144
```

在服务器上执行：

```bash
# 创建 .ssh 目录
mkdir -p ~/.ssh
chmod 700 ~/.ssh

# 编辑 authorized_keys 文件
nano ~/.ssh/authorized_keys
```

将刚才复制的公钥内容粘贴进去，保存退出（Ctrl+O, Enter, Ctrl+X）。

然后设置权限：

```bash
chmod 600 ~/.ssh/authorized_keys
exit
```

## 第三步：测试 SSH 连接

在 Windows 本地测试能否免密登录：

```powershell
ssh -i ~/.ssh/github_actions_deploy root@69.5.23.144
```

如果能成功登录，说明配置正确。

## 第四步：在 GitHub 上配置 Secrets

1. 打开您的 GitHub 仓库
2. 进入 **Settings** → **Secrets and variables** → **Actions**
3. 点击 **New repository secret**
4. 添加以下三个 Secret：

### SSH_PRIVATE_KEY
获取私钥内容：
```powershell
cat ~/.ssh/github_actions_deploy
```
复制全部内容（包括 `-----BEGIN OPENSSH PRIVATE KEY-----` 和 `-----END OPENSSH PRIVATE KEY-----`），粘贴到 Secret 值中。

### SERVER_HOST
值：`69.5.23.144`

### SERVER_USER
值：`root`

## 第五步：在服务器上准备目录

通过 SSH 连接到服务器：

```powershell
ssh root@69.5.23.144
```

执行：

```bash
# 创建部署目录
mkdir -p /var/www/simplebookkeeping

# 安装 .NET 运行时（如果未安装）
wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
apt-get update && apt-get install -y dotnet-runtime-8.0

# 退出
exit
```

## 第六步：推送代码触发部署

```powershell
git add .
git commit -m "Add GitHub Actions deployment"
git push origin main
```

推送到 main 分支后，GitHub Actions 会自动执行部署流程。

## 注意事项

⚠️ **安全警告**：您使用的 root 密码已暴露，请立即更改！

在服务器上执行：
```bash
passwd
```

## 自定义服务管理

如果您的服务器使用 systemd 管理服务，可以创建一个 service 文件：

```bash
# 在服务器上创建
nano /etc/systemd/system/simplebookkeeping.service
```

内容：
```ini
[Unit]
Description=SimpleBookkeeping API
After=network.target

[Service]
WorkingDirectory=/var/www/simplebookkeeping
ExecStart=/usr/bin/dotnet SimpleBookkeeping.Api.dll
Restart=always
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

然后启用服务：
```bash
systemctl daemon-reload
systemctl enable simplebookkeeping
systemctl start simplebookkeeping
```

如果是这样，需要修改 `.github/workflows/deploy.yml` 中的启动部分为：
```yaml
ssh ${{ secrets.SERVER_USER }}@${{ secrets.SERVER_HOST }} << 'EOF'
  cd /var/www/simplebookkeeping
  systemctl restart simplebookkeeping
EOF
```
