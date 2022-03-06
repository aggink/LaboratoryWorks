using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using ProtectionOfInfo.WebApp.Data.Entities.CryptographyEntities;
using ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService
{
    /// <summary>
    ///  // aggink: update summary - 07.03.2022 1:14:58
    /// </summary>
    public class SettingEDSFileService : ISettingEDSFileService
    {
        private const string issuer = "CN=smt ROOT CA";
        private const string subject = "C=GB, ST=Berkshire, L=Reading, O=smtsmt";

        private readonly IWebHostEnvironment _env;
        private readonly string WayPath;

        public SettingEDSFileService(IWebHostEnvironment env)
        {
            _env = env;
            WayPath = Path.Combine(_env.ContentRootPath, "wwwroot", "certificates");
        }

        /// <summary>
        ///  // aggink: update summary - 05.03.2022 13:35:50
        ///  // создание сертификата + выгрузка его данных (сертификат, закрытый ключ, открытый ключ)
        /// </summary>
        /// <param name="months"></param>
        /// <returns></returns>
        public async Task<MyFileInfo?> CreateСertificateAsync(int months)
        {
            string directoryName = Guid.NewGuid().ToString();
            string WayDirectory = Path.Combine(WayPath, directoryName);

            string WayCert = Path.Combine(WayDirectory, "Cert.der");
            string WayPublicKey = Path.Combine(WayDirectory, "PrivateKey.pem");
            string WayPrivateKey = Path.Combine(WayDirectory, "PublicKey.pem");

            string WayZip = Path.Combine(WayPath, $"{directoryName}.zip");

            try
            {
                AsymmetricCipherKeyPair CertificateKey;

                //let us first generate the root certificate
                X509Certificate2 X509RootCert = CreateCertificate(subject, issuer, months, out CertificateKey);
                var Cert = X509RootCert.RawData;
                Directory.CreateDirectory(WayDirectory);
                await File.WriteAllBytesAsync(WayCert, Cert);

                //now let us also create the PEM file as well in case we need it
                using (TextWriter textWriter = new StreamWriter(WayPrivateKey, false))
                {
                    PemWriter pemWriter = new PemWriter(textWriter);
                    pemWriter.WriteObject(CertificateKey.Public);
                    pemWriter.Writer.Flush();
                }

                //now let us also create the PEM file as well in case we need it
                using (TextWriter textWriter = new StreamWriter(WayPublicKey, false))
                {
                    PemWriter pemWriter = new PemWriter(textWriter);
                    pemWriter.WriteObject(CertificateKey.Private);
                    pemWriter.Writer.Flush();
                }

                ZipFile.CreateFromDirectory(WayDirectory, WayZip);
                Directory.Delete(WayDirectory, true);

                var archive = await File.ReadAllBytesAsync(WayZip);
                File.Delete(WayZip);

                return new MyFileInfo(archive, "Сertificate.zip", "zip");
            }
            catch (Exception ex)
            {
                if (Directory.Exists(WayDirectory))
                {
                    Directory.Delete(WayDirectory, true);
                }

                if (File.Exists(WayZip))
                {
                    File.Delete(WayZip);
                }
                return null;
            }
        }

        public async Task<MyFileInfo?> SignFileAsync(IFormFile uploadedFile, IFormFile privateKey, string password)
        {
            if(Path.GetExtension(privateKey.FileName) != ".pem") return null;

            string directoryName = Guid.NewGuid().ToString();
            string WayDirectory = Path.Combine(WayPath, directoryName);

            string uploadedFileName = Path.Combine(WayDirectory, $"{Guid.NewGuid()}.txt");
            string privateKeyFileName = Path.Combine(WayDirectory, $"{Guid.NewGuid()}.pem");

            long origFileLength = 0;

            try 
            { 
                Directory.CreateDirectory(WayDirectory);

                //сохранение файла для подписания на сервере
                string uploadFileExt = Path.GetExtension(uploadedFile.FileName);
                using (var uploadedFileStream = uploadedFile.OpenReadStream())
                {
                    origFileLength = uploadedFile.Length;
                    byte[] fileData = new byte[uploadedFile.Length];
                    await uploadedFileStream.ReadAsync(fileData);
                    await File.WriteAllBytesAsync(uploadedFileName, fileData);
                }

                //сохранение закрытого ключа на сервере
                using (var PrivateKeyStream = privateKey.OpenReadStream())
                {
                    byte[] Key = new byte[privateKey.Length];
                    await PrivateKeyStream.ReadAsync(Key);
                    await File.WriteAllBytesAsync(privateKeyFileName, Key);
                }

                // make sign
                string sign = RSASigntWithPEMPrivateKey(privateKeyFileName, password);

                using (StreamWriter sw = File.AppendText(uploadedFileName))
                {
                    // to end of file put sign
                    sw.Write($"\n{sign}");
                    // to end of file put original extension
                    sw.Write($"\n{uploadFileExt}");
                    sw.Write($"\n{origFileLength}");
                }

                //переводим подписанный файл в байты
                MyFileInfo signed = new MyFileInfo(await File.ReadAllBytesAsync(uploadedFileName), "FileSign.sign", "sign");

                //удаляем созданные файлы на сервере
                Directory.Delete(WayDirectory, true);

                return signed;
            }
            catch (Exception ex)
            {
                if (Directory.Exists(WayDirectory))
                {
                    Directory.Delete(WayDirectory, true);
                }

                return null;
            }
        }

        /// <summary>
        ///  // aggink: update summary - 05.03.2022 14:55:23
        ///  // получить оригинальный файл
        /// </summary>
        /// <param name="uploadedFile"></param>
        /// <returns></returns>
        public async Task<MyFileInfo?> GetOriginalFileAsync(IFormFile uploadedFile)
        {
            if (Path.GetExtension(uploadedFile.FileName) != ".sign") return null;

            string directoryName = Guid.NewGuid().ToString();
            string WayDirectory = Path.Combine(WayPath, directoryName);

            string uploadFileName = Path.Combine(WayDirectory, $"{Guid.NewGuid()}.sign");
            string originalFileName = Path.Combine(WayDirectory, $"{Guid.NewGuid()}");

            try
            {
                Directory.CreateDirectory(WayDirectory);

                using (var stream = uploadedFile.OpenReadStream())
                {
                    byte[] uploadedfileData = new byte[uploadedFile.Length];
                    await stream.ReadAsync(uploadedfileData);
                    await File.WriteAllBytesAsync(uploadFileName, uploadedfileData);
                }

                var uploadedFileList = (await File.ReadAllLinesAsync(uploadFileName)).ToList();
                var uploadedFileBytes = await File.ReadAllBytesAsync(uploadFileName);
                var extension = uploadedFileList[uploadedFileList.Count - 2];
                var origFileLength = long.Parse(uploadedFileList[uploadedFileList.Count - 1]);

                originalFileName += extension;
                byte[] origFile = new byte[origFileLength];
                Array.Copy(uploadedFileBytes, 0, origFile, 0, origFileLength);

                Directory.Delete(WayDirectory, true);

                return new MyFileInfo(origFile, $"OriginalFile{extension}", extension.Trim('.'));
            }
            catch (Exception ex)
            {
                if (Directory.Exists(WayDirectory))
                {
                    Directory.Delete(WayDirectory, true);
                }

                return null;
            }
        }

        /// <summary>
        ///  // aggink: update summary - 05.03.2022 20:46:24
        ///  // проверить подпись к файлу
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckSignFileAsync(IFormFile uploadedFile, IFormFile publicKey, string password)
        {
            if (Path.GetExtension(uploadedFile.FileName) != ".sign") return false;
            if (Path.GetExtension(publicKey.FileName) != ".pem") return false;

            string directoryName = Guid.NewGuid().ToString();
            string WayDirectory = Path.Combine(WayPath, directoryName);

            string uploadFileName = Path.Combine(WayDirectory, $"{Guid.NewGuid()}.sign");
            string publicKeyName = Path.Combine(WayDirectory, $"{Guid.NewGuid()}.pem");

            try
            {
                Directory.CreateDirectory(WayDirectory);

                using (var uploadedFileStream = uploadedFile.OpenReadStream())
                {
                    byte[] fileData = new byte[uploadedFile.Length];
                    await uploadedFileStream.ReadAsync(fileData);
                    await File.WriteAllBytesAsync(uploadFileName, fileData);
                }

                using (var publicKeyStream = publicKey.OpenReadStream())
                {
                    byte[] key = new byte[publicKey.Length];
                    await publicKeyStream.ReadAsync(key);
                    await File.WriteAllBytesAsync(publicKeyName, key);
                }

                List<string> sign = (await File.ReadAllLinesAsync(uploadFileName)).Where(x => x.EndsWith("==")).ToList();
                bool Result = VerifySignature(publicKeyName, password, sign.Last());

                Directory.Delete(WayDirectory, true);

                return Result;
            }
            catch (Exception ex)
            {
                if (Directory.Exists(WayDirectory))
                {
                    Directory.Delete(WayDirectory, true);
                }

                return false;
            }
        }

        private bool VerifySignature(string PublicKeyPEMFileName, string Text, string ExpectedSignature)
        {
            byte[] BytesToSign = Encoding.UTF8.GetBytes(Text);
            byte[] ExpectedSignatureBytes = Convert.FromBase64String(ExpectedSignature);

            AsymmetricKeyParameter KeyPair;
            using (TextReader reader = File.OpenText(PublicKeyPEMFileName))
            {
                KeyPair = (AsymmetricKeyParameter)new PemReader(reader).ReadObject();
            }

            Sha256Digest sha256Digest = new Sha256Digest();
            byte[] TheHash = new byte[sha256Digest.GetDigestSize()];
            sha256Digest.BlockUpdate(BytesToSign, 0, BytesToSign.Length);
            sha256Digest.DoFinal(TheHash, 0);

            PssSigner Signer = new PssSigner(new RsaEngine(), new Sha256Digest(), sha256Digest.GetDigestSize());
            Signer.Init(false, KeyPair);
            Signer.BlockUpdate(TheHash, 0, TheHash.Length);
            return Signer.VerifySignature(ExpectedSignatureBytes);
        }

        private string RSASigntWithPEMPrivateKey(string PrivateKeyPEMFileName, string password)
        {
            byte[] BytesToSign = Encoding.UTF8.GetBytes(password);
            AsymmetricCipherKeyPair KeyPair;
            byte[] Signature;

            using (TextReader reader = File.OpenText(PrivateKeyPEMFileName))
            {
                KeyPair = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();
                Signature = RSASigntWithPrivateKey(KeyPair, BytesToSign);
            }

            string Result = Convert.ToBase64String(Signature);
            return Result;
        }

        private byte[] RSASigntWithPrivateKey(AsymmetricCipherKeyPair KeyPair, byte[] BytesToSign)
        {
            // compute the SHA 256 hash from the bytes to sign received
            Sha256Digest sha256Digest = new Sha256Digest();
            byte[] TheHash = new byte[sha256Digest.GetDigestSize()];
            sha256Digest.BlockUpdate(BytesToSign, 0, BytesToSign.Length);
            sha256Digest.DoFinal(TheHash, 0);

            PssSigner Signer = new PssSigner(new RsaEngine(), new Sha256Digest(), sha256Digest.GetDigestSize());
            Signer.Init(true, KeyPair.Private);
            Signer.BlockUpdate(TheHash, 0, TheHash.Length);
            byte[] Signature = Signer.GenerateSignature();

            return Signature;
        }

        private X509Certificate2 CreateCertificate(string subjectName, string issuer, int ValidMonths, out AsymmetricCipherKeyPair KeyPair, int keyStrength = 2048)
        {
            // Generating Random Numbers
            CryptoApiRandomGenerator randomGenerator = new();
            var random = new SecureRandom(randomGenerator);

            // The Certificate Generator
            X509V3CertificateGenerator certificateGenerator = new();

            // Serial Number
            var serialNumber = BigIntegers.CreateRandomInRange(Org.BouncyCastle.Math.BigInteger.One, Org.BouncyCastle.Math.BigInteger.ValueOf(Int64.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);

            // Issuer and Subject Name
            var subjectDN = new X509Name(subjectName);
            var issuerDN = new X509Name(issuer);
            certificateGenerator.SetIssuerDN(issuerDN);
            certificateGenerator.SetSubjectDN(subjectDN);

            // Valid For
            var notBefore = DateTime.UtcNow.Date;
            var notAfter = notBefore.AddMonths(ValidMonths);

            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(notAfter);

            certificateGenerator.AddExtension(X509Extensions.KeyUsage.Id, true, new KeyUsage(KeyUsage.KeyEncipherment));

            // Subject Public Key
            AsymmetricCipherKeyPair subjectKeyPair;
            var keyGenerationParameters = new KeyGenerationParameters(random, keyStrength);
            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            subjectKeyPair = keyPairGenerator.GenerateKeyPair();

            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            // Generating the Certificate
            var issuerKeyPair = subjectKeyPair;
            KeyPair = subjectKeyPair;

            // Selfsign certificate
            certificateGenerator.SetSignatureAlgorithm("SHA256WithRSA");
            var certificate = certificateGenerator.Generate(issuerKeyPair.Private, random);
            certificate.CheckValidity();
            var x509 = new X509Certificate2(certificate.GetEncoded());

            return x509;
        }
    }
}
