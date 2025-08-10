[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Language: C++](https://img.shields.io/badge/Lang-C%2B%2B-lightgrey.svg)](https://isocpp.org)
[![Language: C%23](https://img.shields.io/badge/Lang-C%23-blue.svg)](https://docs.microsoft.com/dotnet/csharp/)
[![WinUI 3](https://img.shields.io/badge/Framework-WinUI%203-blueviolet.svg)](https://docs.microsoft.com/windows/apps/winui/)

<p align="center">
<img width="2338" height="897" alt="Снимок экрана 2025-08-11 003554" src="https://github.com/user-attachments/assets/efd99090-de9d-43cb-860d-0399e285c90f" />

</p>


# WinUI3 Process Explorer 🚀

*A vibrant process management tool leveraging **WinUI 3**, **C#**, and **C++**.*

---

## 📖 Table of Contents

1. [Status & Version](#status--version)
2. [Branches](#branches)
3. [Features](#features)
4. [Screenshots](#screenshots)
5. [Overview](#overview)
6. [Pages](#pages)
8. [Getting Started](#getting-started)

---

## 📋 Status & Version

* **Status:** In Development 🚧
* **Version:** Alpha 🅰️

---

## 🌿 Branches

| Branch            | Description                                                       |
| ----------------- | ----------------------------------------------------------------- |
| **main (master)** | Secure and stable for general use                                 |
| **experimental**  | NTAPI-based build for deep system access (unstable, research use) |

---

## ✨ Features

<ul>
  <li>⚙️ <strong>Process Metrics</strong>: Live CPU, memory </li>
  <li>🔨 <strong>Process Actions</strong>: Start, suspend, resume & kill processes.</li>
  <li>🛠️ <strong>Extensible C++ Backend</strong>: Low-level Win32 & NTAPI hooks.</li>
  <li>🤖 <strong>Experimental Mode</strong>: Deep-dive NTAPI support.</li>
</ul>

---

## 📸 Screenshots

<p align="center">
  <img width="2879" height="1521" alt="Снимок экрана 2025-08-05 131532" src="https://github.com/user-attachments/assets/9f43a0fd-3422-4e97-8845-0fda091ada4f" />
  <img width="2879" height="1517" alt="Снимок экрана 2025-08-05 131521" src="https://github.com/user-attachments/assets/fa341e48-fa33-44ef-9cfc-5a3c5fad8223" />
</p>

---

## 📝 Overview

Dive into Windows process internals with a two-tab interface. **Metrics** gives you a quick glance at resource usage, while **Actions** empowers you to manage processes in real-time. The experimental branch uses NTAPI for even deeper control.

> ⚠️ **Early Alpha:** Expect rapid changes! Many components will evolve as development progresses.

---

## 📑 Pages

| Page        | Icon | Description                                           |
| ----------- | ---- | ----------------------------------------------------- |
| **Metrics** | 📊   | Aggregate CPU, memory, GPU stats across all processes |
| **Actions** | 🎛️  | Launch, suspend, resume, terminate processes          |


## 🏁 Getting Started

### Prerequisites

* Windows 10 (1903+) / Windows 11
* Visual Studio 2022 (WinUI 3 & C++ workloads)

### Quick Install

```bash
git clone https://github.com/your-user/WinUI-ProcessExplorer.git
cd WinUI-ProcessExplorer
nuget restore
start WinUIProcessExplorer.sln
# Build & Run
```

