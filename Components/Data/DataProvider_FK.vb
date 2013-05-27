'
' Bring2mind - http://www.bring2mind.net
' Copyright (c) 2013
' by Bring2mind
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports System
Imports DotNetNuke

Namespace Data

 Partial Public MustInherit Class DataProvider

#Region " BlogPermission Methods "
  Public MustOverride Function GetBlogPermissionsByBlog(blogID As Int32, StartRowIndex As Integer, MaximumRows As Integer, OrderBy As String) As IDataReader
#End Region

#Region " Blog Methods "
  Public MustOverride Function GetBlogsByCreatedByUser(userID As Int32, StartRowIndex As Integer, MaximumRows As Integer, OrderBy As String) As IDataReader
#End Region

#Region " Comment Methods "
  Public MustOverride Function GetCommentsByContentItem(contentItemId As Int32, IncludeNonApproved As Boolean) As IDataReader
  Public MustOverride Function GetCommentsByModuleId(moduleId As Int32) As IDataReader
#End Region

#Region " Post Methods "
  Public MustOverride Function GetPostsByBlog(blogID As Int32, displayLocale As String, pageIndex As Int32, pageSize As Int32, orderBy As String) As IDataReader
#End Region

 End Class

End Namespace

