# BIRT default encrypted connection profile editor

Tool to decypt, edit and re-encrypt BIRT Connection Profile Files (DataSource).

![howto](birt_deconp_editor/assets/howto.gif)

This is a **proof of concept**, no final product!  

## Motivation

BIRT Eclipse Plugin gets less frequently updates to support the latest Library/IDE/Java Versions (there is now a [Fork](https://github.com/Flugtiger/birt)), slow to use, heavy setup, not as portable as single .exe file.  
The Default Configuration of BIRT that is propably mostly used (enabled through a checkbox within Eclipse BIRT Plugin at File creation), uses a symmetrical device indepened encryption (key). (security by obscurity)  

## Research

- [Stackoverflow: Can we encrypt the BIRT Connection Profile without Eclipse IDE?](https://stackoverflow.com/questions/62169881/can-we-encrypt-the-birt-connection-profile-without-eclipse-ide)
- [Eclipse Forum: encryption method for connection profile files](https://www.eclipse.org/forums/index.php/t/152213/)
  > The encryption is configurable but uses the java crypto packages.  
    The default version has the key embedded in the plugin and is defined in the DefaultCipherProvider class in  
    the DTP projects org.eclipse.datatools.connectivity project.  
    It is called by the ConnectionProfileMgmt class in the same project.  
    Are you using a php->java bridge?  
    If so you may be able to use similar code to write these profiles.  
- [Eclipse BugTracker: How BIRT Encrypts the Connection Profile file](https://bugs.eclipse.org/bugs/show_bug.cgi?id=202648#c11)
  > BIRT simply uses the built-in encryption capabilities (DefaultCipherProvider implementation) provided by DTP connectivity.  
  It is triggered by the checkbox control for "Encrypt file content" in the "Export Connection Profiles" dialog.
- [Eclipse Forum: BIRT encryption algorithm - FIPS compliance](https://www.eclipse.org/forums/index.php/t/119182/)
  > The algorithm should not be a problem, the secret key however should be kept -ehm- secret :-)
- [Stackoverflow: BIRT report how to decrypt password from db and show](https://stackoverflow.com/questions/61672890/birt-report-how-to-decrypt-password-from-db-and-show/61723876#61723876)
  > IF you mean the DB password that is stored in the properties of a DataSource in a *.rptdesign* or *.rptlibrary* file: This is only base64-encrypted (slightly better than clear text). So decrypting it is easy.
- Source Code  
  - DTP Source Code
    - [Eclipse Projekt Info](http://wiki.eclipse.org/DTP_Source_Code_Repository)
    - Git Repo: [org.eclipse.datatools](https://git.eclipse.org/c/datatools/org.eclipse.datatools.git/)  
      `clone git://git.eclipse.org/gitroot/datatools/org.eclipse.datatools.git`
  - Important Files:  
    - DefaultCipherProvider.java  
      `plugins\connectivity\org.eclipse.datatools.connectivity\src\org\eclipse\datatools\connectivity\internal\security\DefaultCipherProvider.java`  
    - cpkey
      `plugins\connectivity\org.eclipse.datatools.connectivity\src\org\eclipse\datatools\connectivity\internal\security\cpkey`
    - ConnectionProfileMgmt.java  
      `plugins\connectivity\org.eclipse.datatools.connectivity\src\org\eclipse\datatools\connectivity\internal\ConnectionProfileMgmt.java`  
    - ImportProfilesDialog.java  
      `plugins\connectivity\org.eclipse.datatools.connectivity.ui\src\org\eclipse\datatools\connectivity\internal\ui\wizards\ImportProfilesDialog.java`
  - The File `cpkey` in imported through `ObjectInputStream()` and `readObject()`.  
    The `SecretKeySpec` is converted with `generateSecret()` (`SecretKeyFactory`) to a Class `SecretKey`
    - The File contains the Code Objekt (Instance) with the Key.  
  - With the Help of `cpkeyExampleCreate.java` its possible to Create a similar File but with a known Key.
    The Comparison (through Hex Editor) reveals the unknown Key as the follwing:
    `EA F1 57 FB FD F2 6E 0E 3B 9D C8 7F 16 0B 91 25 EA F1 57 FB FD F2 6E 0E`
    - Example: The File `cpkeyExample` contains the Key `Example`, compaired with `cpkey` the Key is (`cpkey24Byte.hex`)  based on the differences
    - Additional: `loadKey()` of `DefaultCipherProvider` uses a 24 Byte Key. (see `DESedeKeySpec` Constructor)

## The PoC Application

- .NET Core C# Program developed within Visual Studio Code
  - to create single .exe File (`strg+shift+p` -> `run task` -> `publish`)
- internal Procedure
  - load File
  - DESede decryption with Key
  - change File Content through Editor (notepad.exe) (File Content temporarly enecrypted within Windows default Temp Folder(s))
  - DESede encryption with Key
  - overwrite File
- User interaction
  - File Drag & Drop onto Appliction .exe
  - File Path through Application parameter
- difficulties
  - DESede within C#
    - [Stackoverflow: 3DES (DESede)- Decrypt encrypted text (done by JAVA) in C#](https://stackoverflow.com/questions/36529337/3des-desede-decrypt-encrypted-text-done-by-java-in-c-sharp)  
  - Algorithm details
    - CipherMode `ECB`
    - Padding `PKCS7` ([PKCS5](https://stackoverflow.com/questions/8614997/pkcs5padding-in-c-sharp))
