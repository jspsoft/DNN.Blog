﻿'
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

Imports System.Web
Imports System.Web.Services
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Entities.Portals

Public Class BlogImage
 Implements System.Web.IHttpHandler

 Private Property BlogId As Integer = -1
 Private Property PostId As Integer = -1
 Private Property ModuleId As Integer = -1
 Private Property TabId As Integer = -1
 Private Property Key As String = ""
 Private Property Width As Integer = -1
 Private Property Height As Integer = -1
 Private Property Crop As Boolean = False
 Private Property PortalSettings As PortalSettings = Nothing

 Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

  PortalSettings = PortalController.GetCurrentPortalSettings
  context.Request.Params.ReadValue("Blog", BlogId)
  context.Request.Params.ReadValue("Post", PostId)
  context.Request.Params.ReadValue("Key", Key)
  context.Request.Params.ReadValue("ModuleId", ModuleId)
  context.Request.Params.ReadValue("TabId", TabId)
  context.Request.Params.ReadValue("w", Width)
  context.Request.Params.ReadValue("h", Height)
  context.Request.Params.ReadValue("c", Crop)

  Try
   Dim path As String = ""
   If PostId > -1 Then ' we're looking for an Post's image
    path = GetPostDirectoryMapPath(BlogId, PostId)
   ElseIf BlogId > -1 Then ' we're looking for a blog's image
    path = GetBlogDirectoryMapPath(BlogId)
   End If
   Dim files() As String = IO.Directory.GetFiles(path, String.Format("{0}-{1}-{2}-{3}.*", Key, Width, Height, Crop))
   If files.Length > 0 Then
    context.Response.WriteFile(files(0))
   Else
    files = IO.Directory.GetFiles(path, String.Format("{0}.*", Key))
    If files.Length > 0 Then
     Dim img As New Common.Image(files(0))
     If img.IsValidExtension Then
      Dim newImg As String = img.ResizeImage(Width, Height, Crop)
      context.Response.WriteFile(newImg)
      img.Dispose()
     End If
    End If
   End If
  Catch ex As Exception
  End Try

 End Sub

 ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
  Get
   Return False
  End Get
 End Property

End Class