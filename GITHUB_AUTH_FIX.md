# 🔐 GitHub Authentication Fix - Setup Guide

## Problem
```
remote: Invalid username or token. Password authentication is not supported for Git operations.
fatal: Authentication failed for 'https://github.com/Mandache-Eduard/PSSC.git/'
```

## Solution: Use Personal Access Token (PAT)

GitHub requires one of these for authentication:
1. **Personal Access Token (PAT)** - Recommended, easiest ✅
2. **SSH Keys** - More secure, requires setup
3. **GitHub CLI** - Modern approach

---

## ✅ Option 1: Personal Access Token (EASIEST - Recommended)

### Step 1: Create a Personal Access Token on GitHub

1. Go to: https://github.com/settings/tokens
2. Click **"Generate new token"** → **"Generate new token (classic)"**
3. Fill in:
   - **Token name**: `PSSC-Project` (or any name)
   - **Expiration**: 90 days (or your preference)
   - **Select scopes** (checkboxes): 
     - ✅ `repo` (Full control of private repositories)
4. Click **"Generate token"**
5. **Copy the token immediately** (you won't see it again!)
   - It looks like: `ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx`

### Step 2: Configure Git with Token

```bash
cd C:\Users\manda\OneDrive\Desktop\PSSC

# Option A: Add remote with token in URL (one-liner)
git remote add origin https://YOUR-TOKEN@github.com/Mandache-Eduard/PSSC.git

# Option B: Store credentials in Windows Credential Manager (more secure)
# First, add the remote normally:
git remote add origin https://github.com/Mandache-Eduard/PSSC.git

# Then run this to store credentials:
git config --global credential.helper wincred
# When prompted for password, use your token instead
```

### Step 3: Push Your Code

```bash
cd C:\Users\manda\OneDrive\Desktop\PSSC
git push -u origin main
```

---

## ✅ Option 2: SSH Keys (More Secure)

### Step 1: Generate SSH Key (if you don't have one)

```bash
# Generate new SSH key
ssh-keygen -t ed25519 -C "your-email@example.com"

# When prompted, press Enter to use default location
# Enter passphrase (optional but recommended)
```

Your key will be saved at: `C:\Users\manda\.ssh\id_ed25519`

### Step 2: Add SSH Key to GitHub

1. Copy your public key:
```bash
# Display the public key
type C:\Users\manda\.ssh\id_ed25519.pub
# Or on Windows PowerShell:
cat C:\Users\manda\.ssh\id_ed25519.pub
```

2. Go to: https://github.com/settings/keys
3. Click **"New SSH key"**
4. Paste your public key
5. Click **"Add SSH key"**

### Step 3: Configure Git Remote with SSH

```bash
cd C:\Users\manda\OneDrive\Desktop\PSSC

# Remove old remote if it exists
git remote remove origin

# Add SSH remote
git remote add origin git@github.com:Mandache-Eduard/PSSC.git

# Push code
git push -u origin main
```

---

## ✅ Option 3: GitHub CLI (Modern Approach)

### Step 1: Install GitHub CLI

Download from: https://cli.github.com/

### Step 2: Authenticate

```bash
gh auth login
# Follow prompts to authenticate
```

### Step 3: Push Code

```bash
cd C:\Users\manda\OneDrive\Desktop\PSSC
gh repo create --source=. --remote=origin --push
```

---

## 🎯 Quick Fix (If You Just Want It Done Now)

**Follow these exact steps:**

```powershell
cd C:\Users\manda\OneDrive\Desktop\PSSC

# Step 1: Remove old remote
git remote remove origin

# Step 2: Add new remote (replace YOUR-TOKEN with actual token from Step 1 above)
git remote add origin https://YOUR-TOKEN@github.com/Mandache-Eduard/PSSC.git

# Step 3: Push to GitHub
git push -u origin main

# If successful, you'll see:
# Enumerating objects: XX, done.
# Counting objects: 100% (XX/XX), done.
# ...
# To https://github.com/Mandache-Eduard/PSSC.git
#  * [new branch]      main -> main
# Branch 'main' set up to track remote branch 'main' from 'origin'.
```

---

## 🔍 Verify It Worked

After successful push:

```bash
# Check remote
git remote -v
# Output should show:
# origin  https://...github.com/Mandache-Eduard/PSSC.git (fetch)
# origin  https://...github.com/Mandache-Eduard/PSSC.git (push)

# Check your GitHub repository
# Go to: https://github.com/Mandache-Eduard/PSSC
# You should see all your code and commits!
```

---

## 🚨 Troubleshooting

### If it still fails:

1. **Check Git is installed**: `git --version`
2. **Check your token hasn't expired**: Create a new one
3. **Try without token first**: `git remote add origin https://github.com/Mandache-Eduard/PSSC.git`
4. **Then let git prompt for credentials**: `git push -u origin main`
5. **Use token as password** when prompted

### If you get "repository not found":

- Make sure the repository URL is exactly: `https://github.com/Mandache-Eduard/PSSC.git`
- Check the repository exists and you have push access
- Verify your token has `repo` scope selected

---

## 📝 My Recommendation

**Use Option 1 (Personal Access Token)** - it's the easiest and fastest:
1. Create token on GitHub (5 minutes)
2. Add remote with token (1 minute)
3. Push code (1 minute)
4. Done! ✅

---

**Need help?** Follow the steps above and let me know what happens!

