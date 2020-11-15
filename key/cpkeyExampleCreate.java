////// https://www.tutorialspoint.com/compile_java8_online.php
package com.tutorialspoint;

import java.io.*;
import javax.xml.bind.DatatypeConverter;
import java.security.MessageDigest;
import java.security.spec.KeySpec;
import java.util.Locale;
import java.nio.file.*;

public class ObjectInputStreamDemo {

    /*
     * Testing
     */
    public static void main(String[] args) {
      byte[] bExample = {'e', 'x', 'a', 'm', 'p', 'l', 'e'};
      SecretKeySpec newSKS = new SecretKeySpec(bExample, "DESede");
      try {
        FileOutputStream out = new FileOutputStream("test.txt");
        ObjectOutputStream oout = new ObjectOutputStream(out);
        oout.writeObject(newSKS);
        oout.flush();

        ObjectInputStream ois = new ObjectInputStream(new FileInputStream("test.txt"));

        Path path = Paths.get("test.txt");
        byte[] data = Files.readAllBytes(path);
        System.out.println(toHexString(data));
      } catch (Exception ex) {
        ex.printStackTrace();
      }
    }
   
    public static String toHexString(byte[] array) {
        return DatatypeConverter.printHexBinary(array);
    }

    /*
     * BIRT Class Structure/Code
     */
    public static class SecretKeySpec implements KeySpec, SecretKey{
        private static final long serialVersionUID = 6577238317307289933L;
        public byte[] key;
        public String algorithm;

        public SecretKeySpec(byte[] key, String algorithm) {
            this.key = key;
            this.algorithm = algorithm;
        }

        public String getAlgorithm() {
            return this.algorithm;
        }

        public String getFormat() {
            return "RAW";
        }

        public byte[] getEncoded() {
            return this.key;
        }

        public boolean equals(Object obj) {
            byte[] thatKey = ((SecretKey)obj).getEncoded();
            return MessageDigest.isEqual(this.key, thatKey);
        }
    }

    public interface KeySpec { }
    public interface SecretKey extends java.security.Key { }
}