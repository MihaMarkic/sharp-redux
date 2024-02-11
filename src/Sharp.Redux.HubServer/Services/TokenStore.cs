using LiteDB;
using Sharp.Redux.HubServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharp.Redux.HubServer.Services
{
    public class TokenStore : ITokenStore
    {
        readonly ILiteCollection<SharpReduxToken> tokens;
        readonly static char[] IdChars;
        readonly static Random random = new Random();
        readonly static object sync = new object();
        static TokenStore()
        {
            List<char> chars = new List<char>();
            for (int i = 'a'; i <= 'z'; i++)
            {
                chars.Add((char)i);
            }
            for (int i = 'A'; i <= 'Z'; i++)
            {
                chars.Add((char)i);
            }
            for (int i = '0'; i <= '9'; i++)
            {
                chars.Add((char)i);
            }
            IdChars = chars.ToArray();
        }
        public TokenStore(LiteDatabase db)
        {
            tokens = db.GetCollection<SharpReduxToken>();
            tokens.EnsureIndex(u => u.Id, unique: true);
            tokens.EnsureIndex(u => u.ProjectId);
        }
        public SharpReduxToken AddReadToken(Guid projectId)
        {
            var token = CreateToken(projectId);
            token.IsRead = true;
            tokens.Insert(token);
            return token;
        }
        public SharpReduxToken AddWriteToken(Guid projectId)
        {
            var token = CreateToken(projectId);
            token.IsWrite = true;
            tokens.Insert(token);
            return token;
        }
        internal static SharpReduxToken CreateToken(Guid projectId)
        {
            return new SharpReduxToken
            {
                Id = GetRandomString(20),
                ProjectId = projectId,
            };
        }
        internal static string GetRandomString(int length)
        {
            lock(sync)
            {
                var chars = new char[length];
                for (int i=0; i<length;i++)
                {
                    chars[i] = IdChars[random.Next(IdChars.Length)];
                }
                return new string(chars);
            }
        }

        public SharpReduxToken Get(string id)
        {
            return tokens.FindById(id);
        }
        public SharpReduxToken[] GetForProject(Guid projectId)
        {
            return tokens.Find(t => t.ProjectId == projectId).ToArray();
        }
    }
}
