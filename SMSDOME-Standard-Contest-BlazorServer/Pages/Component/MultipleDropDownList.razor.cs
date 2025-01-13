using Entities.Component;
using Microsoft.AspNetCore.Components;

namespace SMSDOME_Standard_Contest_BlazorServer.Pages.Component
{
    public partial class MultipleDropDownList<T> where T : class
    {
        [Parameter]
        public List<DropDownItemData<T>> DataSource { get; set; } = new List<DropDownItemData<T>>();
        [Parameter]
        public string Value { get; set; } = "";
        [Parameter]
        public string Label { get; set; } = "Data";
        [Parameter]
        public EventCallback<List<DropDownItemData<T>>> ValueChanged { get; set; }


        private string Text;
        private bool Active { get; set; } = false;
        private string Search { get; set; }
        private List<DropDownItemData<T>> SearchDataSource { get; set; } = new List<DropDownItemData<T>>();
        private List<DropDownItemData<T>> SelectedItems { get; set; } = new List<DropDownItemData<T>>();
        protected override void OnParametersSet()
        {
            SearchDataSource = DataSource;
        }

        private void OnClickedItem(DropDownItemData<T> item)
        {
            var isSelectedItem = SelectedItems.Where(p => p.Id == item.Id).FirstOrDefault() == null ? false : true;
            if (isSelectedItem)
            {
                SelectedItems.Remove(item);
            }
            else
            {
                SelectedItems.Add(item);
            }
            Text = string.Join(",", SelectedItems.Select(p => p.Text));
            Value = string.Join(",", SelectedItems.Select(p => p.Code));

            ValueChanged.InvokeAsync(SelectedItems);
        }
        private void SearchChanged(ChangeEventArgs __e)
        {
            Search = __e?.Value?.ToString();
            if (!string.IsNullOrEmpty(Search))
            {
                SearchDataSource = DataSource.Where(p => p.Text.ToLower().Contains(Search.ToLower())).ToList();
            }
            else
            {
                SearchDataSource = DataSource;
            }
        }
    }
}