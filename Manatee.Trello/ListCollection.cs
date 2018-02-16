﻿using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Trello.Exceptions;
using Manatee.Trello.Internal;
using Manatee.Trello.Internal.Caching;
using Manatee.Trello.Internal.DataAccess;
using Manatee.Trello.Internal.Validation;
using Manatee.Trello.Json;

namespace Manatee.Trello
{
	/// <summary>
	/// A read-only collection of lists.
	/// </summary>
	public class ReadOnlyListCollection : ReadOnlyCollection<List>
	{
		private Dictionary<string, object> _additionalParameters;

		/// <summary>
		/// Retrieves a list which matches the supplied key.
		/// </summary>
		/// <param name="key">The key to match.</param>
		/// <returns>The matching list, or null if none found.</returns>
		/// <remarks>
		/// Matches on <see cref="List.Id"/> and <see cref="List.Name"/>.  Comparison is case-sensitive.
		/// </remarks>
		public List this[string key] => GetByKey(key);

		internal ReadOnlyListCollection(Func<string> getOwnerId, TrelloAuthorization auth)
			: base(getOwnerId, auth) {}
		internal ReadOnlyListCollection(ReadOnlyListCollection source, TrelloAuthorization auth)
			: this(() => source.OwnerId, auth)
		{
			if (source._additionalParameters != null)
				_additionalParameters = new Dictionary<string, object>(source._additionalParameters);
		}

		/// <summary>
		/// Implement to provide data to the collection.
		/// </summary>
		protected sealed override void Update()
		{
			IncorporateLimit(_additionalParameters);

			var endpoint = EndpointFactory.Build(EntityRequestType.Board_Read_Lists, new Dictionary<string, object> {{"_id", OwnerId}});
			var newData = JsonRepository.Execute<List<IJsonList>>(Auth, endpoint, _additionalParameters);

			Items.Clear();
			Items.AddRange(newData.Select(jl =>
				{
					var list = jl.GetFromCache<List>(Auth);
					list.Json = jl;
					return list;
				}));
		}

		internal void SetFilter(ListFilter cardStatus)
		{
			if (_additionalParameters == null)
				_additionalParameters = new Dictionary<string, object>();
			_additionalParameters["filter"] = cardStatus.GetDescription();
		}

		private List GetByKey(string key)
		{
			return this.FirstOrDefault(l => key.In(l.Id, l.Name));
		}
	}

	/// <summary>
	/// A collection of lists.
	/// </summary>
	public class ListCollection : ReadOnlyListCollection
	{
		internal ListCollection(Func<string> getOwnerId, TrelloAuthorization auth)
			: base(getOwnerId, auth) { }

		/// <summary>
		/// Creates a new list.
		/// </summary>
		/// <param name="name">The name of the list to add.</param>
		/// <returns>The <see cref="List"/> generated by Trello.</returns>
		public List Add(string name)
		{
			var error = NotNullOrWhiteSpaceRule.Instance.Validate(null, name);
			if (error != null)
				throw new ValidationException<string>(name, new[] { error });

			var json = TrelloConfiguration.JsonFactory.Create<IJsonList>();
			json.Name = name;
			json.Board = TrelloConfiguration.JsonFactory.Create<IJsonBoard>();
			json.Board.Id = OwnerId;

			var endpoint = EndpointFactory.Build(EntityRequestType.Board_Write_AddList);
			var newData = JsonRepository.Execute(Auth, endpoint, json);

			return new List(newData, Auth);
		}
	}
}