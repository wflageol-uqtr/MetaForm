using MetaForm.models;
using System.Collections.Generic;
using System.Text.Json;

namespace MetaForm.Data
{
    public class ListService
    {
        private readonly string filePath = "data.json";
        private List<List> lists;

        public ListService()
        {
            if (File.Exists(filePath))
            {
                var jsonData = File.ReadAllText(filePath);
                lists = JsonSerializer.Deserialize<List<List>>(jsonData) ?? new List<List>();
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
                    item[columnName] = string.Empty;
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
                foreach (var column in list.Columns)
                {
                    if (!item.ContainsKey(column))
                    {
                        item[column] = string.Empty;
                    }
                }
                list.Items.Add(item);
                SaveChanges();
            }
            return Task.CompletedTask;
        }

        public Task RemoveListItemAsync(int listId, Dictionary<string, string> item)
        {
            var list = lists.FirstOrDefault(l => l.Id == listId);
            if (list != null)
            {
                list.Items.Remove(item);
                SaveChanges();
            }
            return Task.CompletedTask;
        }

        public Task DeleteListAsync(int id)
        {
            var list = lists.FirstOrDefault(l => l.Id == id);
            if (list != null)
            {
                lists.Remove(list);
                ReassignIds();
                SaveChanges();
            }
            return Task.CompletedTask;
        }

        private void ReassignIds()
        {
            for (int i = 0; i < lists.Count; i++)
            {
                lists[i].Id = i + 1;
            }
        }

        private void SaveChanges()
        {
            var jsonData = JsonSerializer.Serialize(lists);
            File.WriteAllText(filePath, jsonData);
        }
    }
}
