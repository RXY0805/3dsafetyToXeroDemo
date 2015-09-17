using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

using Xero.Api.Core;
using Xero.Api.Core.Model;
using Xero.Api.Core.Model.Types;
using Xero.Api.Core.Model.Status;
using _3dsafetyToXeroDemo.XeroAPIOperation;

namespace _3dsafetyToXeroDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            btnContactUpdate.Enabled = false;
        }
        private APICore _api;

        public APICore api { get { return _api; } }


        private void btnContact_Click(object sender, EventArgs e)
        {
            var contactWatch = Stopwatch.StartNew();
            _api = new APICore() { UserAgent = "3D safety Xero Demo" };
            Contact user = new Contact();
            user.Addresses = new List<Address>();
            user.Phones = new List<Phone>();

            //Phone 
            Phone userPhone = new Phone();
            userPhone.PhoneType = PhoneType.Default;
            userPhone.PhoneNumber = txtPhoneNumber.Text;
            user.Phones.Add(userPhone);

            //Personal detail
            user.FirstName = txtFirstName.Text;
            user.LastName = txtLastName.Text;
            user.Name = txtCompanyName.Text;
            user.EmailAddress = txtEmailAddress.Text;

            XeroAPI xeroapi = new XeroAPI(api);
            txtContactId.Text= xeroapi.ContactCreate(user);
            contactWatch.Stop();
            lblContactTime.Text = "Execute time: " + contactWatch.Elapsed.TotalSeconds.ToString()+ "s";
        }

        private void btnInvoice_Click(object sender, EventArgs e)
        {
            var invoiceTime = Stopwatch.StartNew();
            Invoice invoice = new Invoice();
            invoice.LineItems = new List<LineItem>();

            LineItem item = new LineItem();
            item.Quantity = Convert.ToDecimal(txtQuantity.Text);
            item.UnitAmount = Convert.ToDecimal(txtUnitAmount.Text);

            item.Description = txtInvoiceDescription.Text;
            item.AccountCode = "200";
            item.ItemCode =  txtItemCode.Text.Trim();
            invoice.LineItems.Add(item);
            txtPaidAmount.Text = (item.Quantity * item.UnitAmount).ToString();

            invoice.Date = DateTime.Now;
            invoice.DueDate = DateTime.Now.AddDays(3);
            invoice.Type = InvoiceType.AccountsReceivable;
            invoice.Status = InvoiceStatus.Authorised;
            invoice.LineAmountTypes = LineAmountType.Inclusive;

            XeroAPI xeroapi = new XeroAPI(api);
            string contactId = txtContactId.Text;

            lblInvoiceID.Text = xeroapi.InvoiceCreate(invoice, contactId);
            invoiceTime.Stop();
            lblInvoiceTime.Text = "Execute time: " + invoiceTime.Elapsed.TotalSeconds.ToString() + "s";
        }

        private void btnCheckContact_Click(object sender, EventArgs e)
        {
            XeroAPI xeroapi = new XeroAPI(api);
            lblExistedContact.Text = "Result:  " + (xeroapi.ContactSearchById(txtContactId.Text)? " Existed Contact":" Not Found");
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            var paymentTime = Stopwatch.StartNew();
            XeroAPI xeroapi = new XeroAPI(api);
            string invoiceId = lblInvoiceID.Text;
            decimal paidAmount = Convert.ToDecimal(txtPaidAmount.Text);
            xeroapi.InvoicePay(invoiceId, paidAmount);
            paymentTime.Stop();
            lblPaymentTime.Text = "Execute time: " + paymentTime.Elapsed.TotalSeconds.ToString() + "s";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // _api = new APICore() { UserAgent = "3D safety Xero Demo" };
            XeroAPI xeroapi = new XeroAPI(api);
            string contactId = lblInvoiceID.Text;
            xeroapi.PaymentGet(contactId);
        }

        private void txtContactId_TextChanged(object sender, EventArgs e)
        {
            btnContactUpdate.Enabled= !string.IsNullOrWhiteSpace(this.txtContactId.Text);
        }

        private void btnContactUpdate_Click(object sender, EventArgs e)
        {
            XeroAPI xeroapi = new XeroAPI(api);
            Address userAddress = new Address();
            userAddress.AddressType = AddressType.Street;
            userAddress.AddressLine1 = txtAddressLine1.Text;
            userAddress.AddressLine2 = txtAddressLine2.Text;
            userAddress.AddressLine3 = "line3";
            userAddress.AddressLine4 = "line 4";
            userAddress.City = txtSuburb.Text;
            userAddress.Region = txtState.Text;
            userAddress.PostalCode = txtPostCode.Text;
            userAddress.Country = "AUS";
            userAddress.AttentionTo = "sample";

            string contactId = txtContactId.Text;
            lblAddressUpdated.Text= "Address Updated " +(xeroapi.ContactUpdate(contactId, userAddress) ? "Successful":"Failed");
        }
    }
}
