﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Threading.Tasks;

namespace Insthync.MMOG
{
    public partial class SQLiteDatabase
    {
        private bool ReadCharacterBuff(SQLiteRowsReader reader, out CharacterBuff result, bool resetReader = true)
        {
            if (resetReader)
                reader.ResetReader();

            if (reader.Read())
            {
                result = new CharacterBuff();
                result.id = reader.GetString("id");
                result.characterId = reader.GetInt64("characterId").ToString();
                result.type = (BuffType)reader.GetByte("type");
                result.dataId = reader.GetInt32("dataId");
                result.level = reader.GetInt32("level");
                result.buffRemainsDuration = reader.GetFloat("buffRemainsDuration");
                return true;
            }
            result = CharacterBuff.Empty;
            return false;
        }

        public async Task CreateCharacterBuff(string characterId, CharacterBuff characterBuff)
        {
            await ExecuteNonQuery("INSERT INTO characterbuff (id, characterId, type, dataId, level, buffRemainsDuration) VALUES (@id, @characterId, @type, @dataId, @level, @buffRemainsDuration)",
                new SqliteParameter("@id", characterBuff.id),
                new SqliteParameter("@characterId", characterId),
                new SqliteParameter("@type", (byte)characterBuff.type),
                new SqliteParameter("@dataId", characterBuff.dataId),
                new SqliteParameter("@level", characterBuff.level),
                new SqliteParameter("@buffRemainsDuration", characterBuff.buffRemainsDuration));
        }

        public async Task<List<CharacterBuff>> ReadCharacterBuffs(string characterId)
        {
            var result = new List<CharacterBuff>();
            var reader = await ExecuteReader("SELECT * FROM characterbuff WHERE characterId=@characterId",
                new SqliteParameter("@characterId", characterId));
            CharacterBuff tempBuff;
            while (ReadCharacterBuff(reader, out tempBuff, false))
            {
                result.Add(tempBuff);
            }
            return result;
        }

        public async Task DeleteCharacterBuffs(string characterId)
        {
            await ExecuteNonQuery("DELETE FROM characterbuff WHERE characterId=@characterId", new SqliteParameter("@characterId", characterId));
        }
    }
}
