namespace ProtectionOfInfo.WebApp.Data.CryptographyEntities
{
    public class Certificate
    {
        public Certificate(byte[] cert, byte[] privateKey, byte[] publicKey)
        {
            Cert = cert;
            PrivateKey = privateKey;
            PublicKey = publicKey;
            CertFileName = "X509Cert.der";
            PublicKeyFileName = "X509Cert-public.pem";
            PrivateKeyFileName = "X509Cert-private.pem";
            CertExtension = "der";
            PrivateKeyExtension = "pem";
            PublicKeyExtension = "pem";
        }

        public Certificate(string certFileName, string certExtension, byte[] cert, string privateKeyFileName, string privateKeyExtension, byte[] privateKey, string publicKeyFileName, string publicKeyExtension, byte[] publicKey)
        {
            CertFileName = certFileName;
            CertExtension = certExtension;
            Cert = cert;
            PrivateKeyFileName = privateKeyFileName;
            PrivateKeyExtension = privateKeyExtension;
            PrivateKey = privateKey;
            PublicKeyFileName = publicKeyFileName;
            PublicKeyExtension = publicKeyExtension;
            PublicKey = publicKey;
        }

        public string CertFileName { get; set; }
        public string CertExtension { get; set; }
        public byte[] Cert { get; set; }
        public string PrivateKeyFileName { get; set; }
        public string PrivateKeyExtension { get; set; }
        public byte[] PrivateKey { get; set; }
        public string PublicKeyFileName { get; set; }
        public string PublicKeyExtension { get; set; }
        public byte[] PublicKey { get; set; }
    }
}
