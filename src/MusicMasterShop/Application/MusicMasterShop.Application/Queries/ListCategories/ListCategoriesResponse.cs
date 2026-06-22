using System;
using System.Collections.Generic;
using System.Text;

namespace MusicMasterShop.Application.Queries.ListCategories
{
    public record ListCategoriesResponse(string Nome, int TipoCategoriaId)
    {
    }
}
