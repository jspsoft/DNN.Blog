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

Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports System.Linq
Imports DotNetNuke.Modules.Blog.Entities.Posts
Imports DotNetNuke.Web.UI.WebControls
Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Modules.Blog.Common.Globals

Public Class Admin
 Inherits BlogModuleBase

 Private _totalPosts As Integer = -1

 Private Sub Page_Init1(sender As Object, e As System.EventArgs) Handles Me.Init
  AddJavascriptFile("jquery.dynatree.min.js", 60)
  AddCssFile("dynatree.css")
 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  If Not BlogContext.Security.IsEditor Then
   Throw New Exception("You do not have access to this resource. Please check your login status.")
  End If

  If Not Me.IsPostBack Then
   Me.DataBind()
  End If

  DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdEditTagsML, LocalizeString("LeavePage.Confirm"))
  DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdEditCategoriesML, LocalizeString("LeavePage.Confirm"))

 End Sub

 Private Sub cmdEditTagsML_Click(sender As Object, e As System.EventArgs) Handles cmdEditTagsML.Click
  SaveChanges()
  Response.Redirect(EditUrl("TermsEditML"), False)
 End Sub

 Private Sub cmdEditCategoriesML_Click(sender As Object, e As System.EventArgs) Handles cmdEditCategoriesML.Click
  SaveChanges()
  If Settings.VocabularyId > -1 Then
   Response.Redirect(EditUrl("VocabularyId", Settings.VocabularyId.ToString, "TermsEditML"), False)
  End If
 End Sub

 Private Sub cmdCreateVocabulary_Click(sender As Object, e As System.EventArgs) Handles cmdCreateVocabulary.Click
  Settings.VocabularyId = Integration.Integration.CreateNewVocabulary(PortalId).VocabularyId
  Me.DataBind()
 End Sub

 Private Sub ddVocabularyId_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddVocabularyId.SelectedIndexChanged
  Settings.VocabularyId = ddVocabularyId.SelectedValue.ToInt
  Me.DataBind()
 End Sub

 Private Sub cmdCancel_Click(sender As Object, e As System.EventArgs) Handles cmdCancel.Click
  Response.Redirect(DotNetNuke.Common.NavigateURL(TabId), False)
 End Sub

 Private Sub cmdUpdateSettings_Click(sender As Object, e As System.EventArgs) Handles cmdUpdate.Click
  SaveChanges()
  Response.Redirect(DotNetNuke.Common.NavigateURL(TabId), False)
 End Sub

 Private Sub SaveChanges()
  Settings.AllowAttachments = chkAllowAttachments.Checked
  Settings.SummaryModel = CType(ddSummaryModel.SelectedValue.ToInt, SummaryType)
  Settings.AllowMultipleCategories = chkAllowMultipleCategories.Checked
  Settings.AllowWLW = chkAllowWLW.Checked
  Settings.WLWRecentPostsMax = txtWLWRecentPostsMax.Text.ToInt
  Settings.AllowAllLocales = chkAllowAllLocales.Checked
  Settings.VocabularyId = ddVocabularyId.SelectedValue.ToInt
  Settings.ModifyPageDetails = chkModifyPageDetails.Checked
  Settings.RssAllowContentInFeed = chkRssAllowContentInFeed.Checked
  Settings.RssDefaultCopyright = txtRssDefaultCopyright.Text
  Settings.RssDefaultNrItems = Integer.Parse(txtRssDefaultNrItems.Text)
  Settings.RssEmail = txtEmail.Text
  Settings.RssImageHeight = Integer.Parse(txtRssImageHeight.Text)
  Settings.RssImageWidth = Integer.Parse(txtRssImageWidth.Text)
  Settings.RssImageSizeAllowOverride = chkRssImageSizeAllowOverride.Checked
  Settings.RssMaxNrItems = Integer.Parse(txtRssMaxNrItems.Text)
  Settings.RssTtl = Integer.Parse(txtRssTtl.Text)
  Settings.UpdateSettings()
  If treeState.Value <> DotNetNuke.Modules.Blog.Entities.Terms.TermsController.GetCategoryTreeAsJson(Categories) Then
   Dim categoryTree As List(Of Common.DynatreeItem) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of Common.DynatreeItem))(treeState.Value)
   Dim ReturnedIds As New List(Of Integer)
   Dim i As Integer = 1
   For Each rootNode As Common.DynatreeItem In categoryTree
    AddOrUpdateCategory(-1, i, rootNode, ReturnedIds)
    i += 1
   Next
   Dim deleteCategories As New List(Of Entities.Terms.TermInfo)
   For Each t As Entities.Terms.TermInfo In Categories.Values
    If Not ReturnedIds.Contains(t.TermId) Then deleteCategories.Add(t)
   Next
   For Each categoryToDelete As Entities.Terms.TermInfo In deleteCategories
    DotNetNuke.Entities.Content.Common.Util.GetTermController().DeleteTerm(categoryToDelete)
   Next
   Categories = Entities.Terms.TermsController.GetTermsByVocabulary(ModuleId, Settings.VocabularyId, BlogContext.Locale, True) ' clear the cache
  End If
 End Sub

 Private Sub AddOrUpdateCategory(parentId As Integer, viewOrder As Integer, category As Common.DynatreeItem, ByRef returnedIds As List(Of Integer))
  If String.IsNullOrEmpty(category.title) Then Exit Sub
  Dim termId As Integer = -1
  If IsNumeric(category.key) Then termId = Integer.Parse(category.key)
  termId = Data.DataProvider.Instance.SetTerm(termId, Settings.VocabularyId, parentId, viewOrder, category.title, "", UserId)
  returnedIds.Add(termId)
  Dim i As Integer = 1
  For Each subCategory As Common.DynatreeItem In category.children
   AddOrUpdateCategory(termId, i, subCategory, returnedIds)
   i += 1
  Next
 End Sub

 Public Overrides Sub DataBind()
  MyBase.DataBind()

  chkAllowAttachments.Checked = Settings.AllowAttachments
  Try
   ddSummaryModel.Items.FindByValue(CInt(Settings.SummaryModel).ToString).Selected = True
  Catch ex As Exception
  End Try
  cmdEditTagsML.Enabled = BlogContext.IsMultiLingualSite
  cmdEditCategoriesML.Enabled = BlogContext.IsMultiLingualSite And Settings.VocabularyId > -1
  chkAllowMultipleCategories.Checked = Settings.AllowMultipleCategories
  chkAllowWLW.Checked = Settings.AllowWLW
  chkAllowAllLocales.Checked = Settings.AllowAllLocales
  chkModifyPageDetails.Checked = Settings.ModifyPageDetails

  chkRssAllowContentInFeed.Checked = Settings.RssAllowContentInFeed
  txtRssDefaultCopyright.Text = Settings.RssDefaultCopyright
  txtRssDefaultNrItems.Text = Settings.RssDefaultNrItems.ToString
  txtEmail.Text = Settings.RssEmail
  txtRssImageHeight.Text = Settings.RssImageHeight.ToString
  txtRssImageWidth.Text = Settings.RssImageWidth.ToString
  chkRssImageSizeAllowOverride.Checked = Settings.RssImageSizeAllowOverride
  txtRssMaxNrItems.Text = Settings.RssMaxNrItems.ToString
  txtRssTtl.Text = Settings.RssTtl.ToString

  txtWLWRecentPostsMax.Text = Settings.WLWRecentPostsMax.ToString
  ddVocabularyId.Items.Clear()
  ddVocabularyId.DataSource = Common.Globals.GetPortalVocabularies(PortalId)
  ddVocabularyId.DataBind()
  ddVocabularyId.Items.Insert(0, New ListItem(LocalizeString("NoneSpecified"), "-1"))
  Try
   ddVocabularyId.Items.FindByValue(Settings.VocabularyId.ToString).Selected = True
  Catch ex As Exception
  End Try

  treeState.Value = DotNetNuke.Modules.Blog.Entities.Terms.TermsController.GetCategoryTreeAsJson(Categories)

 End Sub

End Class