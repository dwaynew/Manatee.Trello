﻿/***************************************************************************************

	Copyright 2013 Little Crab Solutions

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		OrganizationPermissionLevelType.cs
	Namespace:		Manatee.Trello
	Class Name:		OrganizationPermissionLevelType
	Purpose:		Enumerates known viewing permission levels for an organization
					on Trello.com.

***************************************************************************************/
namespace Manatee.Trello
{
	///<summary>
	/// Enumerates known values for organization permission levels
	///</summary>
	public enum OrganizationPermissionLevelType
	{
		/// <summary>
		/// Not recognized.  May have been created since the current version of this API.
		/// </summary>
		Unknown = -1,
		/// <summary>
		/// Indicates that the organization can only be viewed by its members.
		/// </summary>
		Private,
		/// <summary>
		/// Indicates that anyone (even non-Trello users) may view the organization.
		/// </summary>
		Public
	}
}