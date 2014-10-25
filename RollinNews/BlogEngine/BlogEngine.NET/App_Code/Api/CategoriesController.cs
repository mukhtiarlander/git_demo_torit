using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using RDN.Library.Classes.Error;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class CategoriesController : ApiController
{
	readonly ICategoryRepository repository;

	public CategoriesController(ICategoryRepository repository)
	{
		this.repository = repository;
	}

	public IEnumerable<BlogEngine.Core.Data.Models.CategoryItem> Get(int take = 500, int skip = 0, string filter = "", string order = "")
	{
		try
		{
			return repository.Find(take, skip, filter, order);
		}
		catch (UnauthorizedAccessException)
		{
			throw new HttpResponseException(HttpStatusCode.Unauthorized);
		}
		catch (Exception exception)
		{
			ErrorDatabaseManager.AddException(exception, GetType());
			throw new HttpResponseException(HttpStatusCode.InternalServerError);
		}
	}

	public HttpResponseMessage Get(string id)
	{
		try
		{
			var result = repository.FindById(new Guid(id));
			if (result == null)
				return Request.CreateResponse(HttpStatusCode.NotFound);

			return Request.CreateResponse(HttpStatusCode.OK, result);
		}
		catch (UnauthorizedAccessException)
		{
			return Request.CreateResponse(HttpStatusCode.Unauthorized);
		}
		catch (Exception exception)
		{
			ErrorDatabaseManager.AddException(exception, GetType());
			return Request.CreateResponse(HttpStatusCode.InternalServerError);
		}
	}

	public HttpResponseMessage Post([FromBody]CategoryItem item)
	{
		try
		{
			var result = repository.Add(item);
			if (result == null)
				return Request.CreateResponse(HttpStatusCode.NotFound);

			return Request.CreateResponse(HttpStatusCode.Created, result);
		}
		catch (UnauthorizedAccessException)
		{
			return Request.CreateResponse(HttpStatusCode.Unauthorized);
		}
		catch (Exception exception)
		{
			ErrorDatabaseManager.AddException(exception, GetType());
			return Request.CreateResponse(HttpStatusCode.InternalServerError);
		}
	}

	[HttpPut]
	public HttpResponseMessage Update([FromBody]CategoryItem item)
	{
		try
		{
			repository.Update(item);
			return Request.CreateResponse(HttpStatusCode.OK);
		}
		catch (UnauthorizedAccessException)
		{
			return Request.CreateResponse(HttpStatusCode.Unauthorized);
		}
		catch (Exception exception)
		{
			ErrorDatabaseManager.AddException(exception, GetType());
			return Request.CreateResponse(HttpStatusCode.InternalServerError);
		}
	}

	public HttpResponseMessage Delete(string id)
	{
		try
		{
			Guid gId;
			if (Guid.TryParse(id, out gId))
				repository.Remove(gId);
			return Request.CreateResponse(HttpStatusCode.OK);
		}
		catch (UnauthorizedAccessException)
		{
			return Request.CreateResponse(HttpStatusCode.Unauthorized);
		}
		catch (Exception exception)
		{
			ErrorDatabaseManager.AddException(exception, GetType());
			return Request.CreateResponse(HttpStatusCode.InternalServerError);
		}
	}
	
	[HttpPut]
	public HttpResponseMessage ProcessChecked([FromBody]List<CategoryItem> items)
	{
		try
		{
			if (items == null || items.Count == 0)
				throw new HttpResponseException(HttpStatusCode.ExpectationFailed);

			var action = Request.GetRouteData().Values["id"].ToString();

			if (action.ToLower() == "delete")
			{
				foreach (var item in items)
				{
					if (item.IsChecked)
					{
						repository.Remove(item.Id);
					}
				}
			}
			return Request.CreateResponse(HttpStatusCode.OK);
		}
		catch (UnauthorizedAccessException)
		{
			return Request.CreateResponse(HttpStatusCode.Unauthorized);
		}
		catch (Exception exception)
		{
			ErrorDatabaseManager.AddException(exception, GetType());
			return Request.CreateResponse(HttpStatusCode.InternalServerError);
		}
	}
}
