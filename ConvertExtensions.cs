using System;

public static class ClassExtensions
{
	public static class  ConvertExtensions()
	{
		public static List<SelectListItem> ConvertToSelectList<T>(this IEnumerable<T> collection, int selectedValue)
		{
			return (
				from drpItem in collection
				select new SelectListItem
				{
					Name = drpItem.Name,
					VillaId = drpItem.VillaId,
					Villa_Number = drpItem.VillaNumber
				}
			).ToList();
		}
	}
}
