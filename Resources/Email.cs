using System;
using System.IO;
using System.Linq;
using Microsoft.Exchange.WebServices.Data;

namespace Shared.Common.Resources
{
    public class Email
    {
        public EmailMessage EmailItem { get; set; }
        public string BodyAsString { get; set; }
        public string Subject { get; set; }

        public DateTime DateTimeCreated => EmailItem.DateTimeCreated;

        public DateTime DateTimeSent => EmailItem.DateTimeSent;

        public DateTime DateTimeReceived => EmailItem.DateTimeReceived;


        public Email(EmailMessage emailItem)
        {
            EmailItem = emailItem;
            BodyAsString = emailItem.Body.Text;
            Subject = emailItem.Subject;
        }
        
        public void SetAsRead()
        {
            EmailItem.IsRead = true;
            EmailItem.Update(ConflictResolutionMode.AlwaysOverwrite);
        }

/*
        private bool HasAttachment()
        {
            return EmailItem.Attachments.Any();
        }
*/

        public Attachment[] GetAttachments()
        {
            return EmailItem.Attachments.ToArray();
        }

        public string ReadAttachment(Attachment attachment)
        {
            string content;

            if (!(attachment is FileAttachment)){
                return null;
            }

            var fileAttachment = attachment as FileAttachment;
            fileAttachment.Load();
            
            using (var reader = new StreamReader(new MemoryStream(fileAttachment.Content)))
            {
                content = reader.ReadToEnd();
            }
            return content;
        }
    }
}
