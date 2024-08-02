using MetaForm.models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MetaForm.Data
{
    public class ListService
    {
        private readonly string filePath = "data.json";
        private List<List> lists;
        private readonly object fileLock = new object();

        public ListService()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var jsonData = File.ReadAllText(filePath);
                    lists = JsonSerializer.Deserialize<List<List>>(jsonData) ?? new List<List>();
                }
                catch (JsonException)
                {
                    lists = new List<List>();
                }
            }
            else
            {
                lists = new List<List>();
            }
        }

        public Task<List<List>> GetListsAsync()
        {
            return Task.FromResult(lists);
        }

        public Task<List?> GetListAsync(int id)
        {
            var list = lists.FirstOrDefault(l => l.Id == id);
            return Task.FromResult(list);
        }

        public Task AddListAsync(List list)
        {
            list.Id = lists.Any() ? lists.Max(l => l.Id) + 1 : 1;
            lists.Add(list);
            SaveChanges();
            return Task.CompletedTask;
        }

        public Task UpdateListAsync(List list)
        {
            var existingList = lists.FirstOrDefault(l => l.Id == list.Id);
            if (existingList != null)
            {
                existingList.Name = list.Name;
                existingList.Columns = list.Columns;
                existingList.Items = list.Items;
                SaveChanges();
            }
            return Task.CompletedTask;
        }

        public Task AddColumnAsync(int listId, string columnName)
        {
            var list = lists.FirstOrDefault(l => l.Id == listId);
            if (list != null && !list.Columns.Contains(columnName))
            {
                list.Columns.Add(columnName);
                foreach (var item in list.Items)
                {
                    item.Values[columnName] = string.Empty;
                }
                SaveChanges();
            }
            return Task.CompletedTask;
        }

        public Task AddListItemAsync(int listId, Dictionary<string, string> item)
        {
            var list = lists.FirstOrDefault(l => l.Id == listId);
            if (list != null)
            {
                var newItem = new ListItem
                {
                    Id = list.Items.Any() ? list.Items.Max(i => i.Id) + 1 : 1,
                    Values = item
                };
                list.Items.Add(newItem);
                SaveChanges();
            }
            return Task.CompletedTask;
        }

        public Task RemoveListItemAsync(int listId, int itemId)
        {
            var list = lists.FirstOrDefault(l => l.Id == listId);
            if (list != null)
            {
                var item = list.Items.FirstOrDefault(i => i.Id == itemId);
                if (item != null)
                {
                    list.Items.Remove(item);
                    SaveChanges();
                }
            }
            return Task.CompletedTask;
        }

        public Task DeleteListAsync(int id)
        {
            var list = lists.FirstOrDefault(l => l.Id == id);
            if (list != null)
            {
                lists.Remove(list);
                SaveChanges();
            }
            return Task.CompletedTask;
        }

        private void SaveChanges()
        {
            lock (fileLock)
            {
                var jsonData = JsonSerializer.Serialize(lists);
                File.WriteAllText(filePath, jsonData);
            }
        }
    }
}
