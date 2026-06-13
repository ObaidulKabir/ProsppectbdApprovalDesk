namespace ProspectbdApprovalDesk.Application.Security;

public interface IEncryptionService
{
    string EncryptToBase64(string plaintext);
    string DecryptFromBase64(string ciphertextBase64);
}

