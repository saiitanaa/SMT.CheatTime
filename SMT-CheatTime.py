import os
import shutil
import winreg
import re
import time
import sys

def find_SMT():
    try:
        import winreg
        key = winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, r"SOFTWARE\WOW6432Node\Valve\Steam")
        steam_path = winreg.QueryValueEx(key, "InstallPath")[0]
        winreg.CloseKey(key)
    except:
        return None

    libraryfolders = os.path.join(steam_path, "steamapps", "libraryfolders.vdf")
    if not os.path.exists(libraryfolders):
        return None

    paths = [steam_path]
    with open(libraryfolders, 'r', encoding='utf-8') as f:
        content = f.read()
        found_paths = re.findall(r'"path"\s+"([^"]+)"', content)
        paths.extend([p.replace('\\\\', '\\') for p in found_paths])

    for path in paths:
        steam_common = os.path.join(path, "steamapps", "common")
        if os.path.exists(steam_common):
            for folder in os.listdir(steam_common):
                if "Supermarket Together" in folder:
                    return os.path.join(steam_common, folder)
    return None

def inject_SMTCT(): 
    SMT = find_SMT()
    if not SMT:
        print("[-] Game not found!")
        time.sleep(2)
        return
        
    print("[+] Injecting files...")
    
    if os.path.exists("BepInEx"):
        shutil.copytree("BepInEx", os.path.join(SMT, "BepInEx"), dirs_exist_ok=True)
    
    BepInEx_files = [".doorstop_version", "doorstop_config.ini", "winhttp.dll"]
    for f in BepInEx_files:
        if os.path.exists(f):
            shutil.copy2(f, SMT)
            
    plugins = os.path.join(SMT, "BepInEx", "plugins")
    os.makedirs(plugins, exist_ok=True)
    
    if os.path.exists("SMT-CHEATTIME.dll"):
        shutil.copy2("SMT-CHEATTIME.dll", plugins)
        print("[+] Injected Successfully")
        time.sleep(0.4)
        os.system("cls")
        main()
    else:
        print("[-] Error: SMT-CHEATTIME.dll missing!")
        time.sleep(2)
        os.system("cls")

def wipe_SMTCT():
    SMT = find_SMT()
    if not SMT: 
        print("[-] Game not found!")
        return
    
    print("[+] Cleaning...")
    wipe = ["winhttp.dll", "doorstop_config.ini", ".doorstop_version"]
    for f in wipe:
        path = os.path.join(SMT, f)
        if os.path.exists(path): os.remove(path)
        
    bep_path = os.path.join(SMT, "BepInEx")
    if os.path.exists(bep_path):
        shutil.rmtree(bep_path)
    print("[-] Wiped Complete\n")
    time.sleep(0.4)
    os.system("cls")
    main()

def main():
    while True:    
        os.system("cls")
        os.system("title SMT.CheatTime!")
        print(">> SMT.CheatTime Loader! <<")
        print("=================================")
        print("1. [+] Inject Cheat")
        print("2. [-] Clean SuperMarket Together")
        print("3. [+] Check Updates")
        print("4. [-] Exit")
        print("=================================")
        print("[!] - Github : saiitanaa\n")
        user = input(">> ")
        
        if user == "1":
            inject_SMTCT()
        elif user == "2":
            wipe_SMTCT()
        elif user == "3":
            os.system("start https://github.com/saiitanaa/SMT.CheatTime/releases/latest")
        elif user == "4":
            sys.exit()
        else:
            print("Invalid choose...\n")
            time.sleep(1)

if __name__ == "__main__":
    main()