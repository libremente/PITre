﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="TransmissionTemplate_saveNewOwner.aspx.cs" Inherits="NttDataWA.Popup.TransmissionTemplate_saveNewOwner" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container {width: 95%; margin: 0 auto;}
        p {text-align: left;}
        .red {color: #f00;}
        .col-right {float: right; font-size: 0.8em;}
        #txtTitle {width: 100%; height: 3em;}
        ul {list-style: none; margin: 0; padding: 0;}
        ul.li {margin: 0; padding: 0;}
    </style>
    <script type="text/javascript">
        $(function () {
            function charsLeft() {
                var maxLength = <%=maxLength%>;
                var actLength = $("#txtTitle").val().length;
                if (actLength>maxLength) {
                    $("#txtTitle").val($("#txtTitle").val().substring(0, maxLength-1));
                    actLength = maxLength;
                }
                $("#txtTitle_chars").text(maxLength - actLength);
            }
            
            $("#txtTitle").keyup(charsLeft);
            $("#txtTitle").change(charsLeft);
            charsLeft();

            $("#TemplateBtnSave").click(function () {              
                $("#txtTitle_err").remove();
                if ($("#txtTitle").val().length == 0) {
                    $("#txtTitle").focus();
                    $("#txtTitle").before('<span id="txtTitle_err" class="red">Campo obbligatorio</span>');
                    return false;
                }
            });

        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
            <div class="container">
                <div id="rowMessage" runat="server" />
                <p>Per questa trasmissione è attiva l'opzione "Cedi i diritti".<br />
                    <br />
                    Poichè il mittente è il proprietario del documento, selezionare dalla lista seguente l'utente che erediterà la proprietà del documento.<br />
                    <br />
                    La proprietà sarà estesa anche al ruolo dell'utente selezionato.
                </p>
                <p><asp:RadioButtonList ID="rblUsers" runat="server" RepeatLayout="UnorderedList" ClientIDMode="Static" ValidationGroup="rblUsers" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="<br />Selezionare un utente" ControlToValidate="rblUsers" EnableClientScript="false" ForeColor="Red"></asp:RequiredFieldValidator>
                </p>
                <p id="rowTitle" runat="server">
                    <strong>Nome Modello *</strong><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="<br />Campo obbligatorio" ControlToValidate="txtTitle" EnableClientScript="false" ForeColor="Red"></asp:RequiredFieldValidator><br />
                    <cc1:CustomTextArea ID="txtTitle" runat="server" Columns="50" Rows="2" TextMode="MultiLine" ClientIDMode="Static" CssClass="txt_textarea"></cc1:CustomTextArea><br />
                    <span class="col-right">Caratteri rimanenti: <span id="txtTitle_chars"></span></span>
                </p>
                <p id="rowShare" runat="server">
                    <strong>Rendi disponibile</strong><br />
                    <asp:radiobuttonlist id="rblShare" runat="server" RepeatLayout="UnorderedList">
						<asp:ListItem Value="usr" Selected="True">solo a me stesso (@usr@)</asp:ListItem>
						<asp:ListItem Value="grp">a tutto il ruolo (@grp@)</asp:ListItem>
					</asp:radiobuttonlist>
                </p>
            </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
            <cc1:CustomButton ID="TemplateBtnSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Salva" ClientIDMode="Static" 
                onclick="TemplateBtnSave_Click" />
            <cc1:CustomButton ID="TemplateBtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Chiudi" ClientIDMode="Static" 
                onclick="TemplateBtnClose_Click" />
</asp:Content>