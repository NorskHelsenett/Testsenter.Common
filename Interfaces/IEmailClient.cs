using System;
using System.Collections.Generic;

namespace Shared.Common.Interfaces
{
    public interface IEmailClient
    {
        void Send(string subject, string content, string from, bool saveCopy, params string[] to);

        IEnumerable<Tuple<DateTime, string>> ReadEmailReturnFirstAttachement(string queryString, string subfolderName, bool setMailAsRead);
        List<Common.Resources.Email> ReadEmail(string queryString, string subfolderName, bool setMailAsRead);
    }
}
