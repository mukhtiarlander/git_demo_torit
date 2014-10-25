<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="GCheckout" %>
<%@ Import Namespace="GCheckout.Util" %>
<%@ Import Namespace="RDN.Raspberry.Models.Utilities" %>
<script runat="server" language="c#">

  public void Page_Load(Object sender, EventArgs e) {
      StreamWriter writer = new StreamWriter(@"C:\temp\payment_test.txt", false);
      
      try{
          string serialNumber = null;
          List<string> data = new List<string>();
      
          // Receive Request
          Stream requestInputStream = Request.InputStream;
          data.Add("Stream length: " + requestInputStream.Length.ToString());
          data.Add("Total bytes: " + Request.TotalBytes.ToString());
          string requestStreamAsString = null;
          using (System.IO.StreamReader oneStreamReader = new StreamReader(requestInputStream))
          {          
              requestStreamAsString = oneStreamReader.ReadToEnd();
          }

          // Parse Request to retreive serial number
          string[] requestStreamAsParts = requestStreamAsString.Split(new char[] { '=' });
          data.Add("Stream: " + requestStreamAsString);
          if (requestStreamAsParts.Length >= 2)
          {
              data.Add("Part1: " + requestStreamAsParts[0]);
              data.Add("Serial: " + requestStreamAsParts[1]);
              serialNumber = requestStreamAsParts[1];
          }

            
          // Call NotificationHistory Google Checkout API to retrieve the notification for the given serial number and process the notification
          GoogleCheckoutHelper.ProcessNotification(serialNumber, ref data);

          //serialize the message to the output stream only if you could process the message.
          //Otherwise throw an http 500.

          var response = new GCheckout.AutoGen.NotificationAcknowledgment();
          response.serialnumber = serialNumber;
          data.Add("Serial: " + response.serialnumber);
      
          HttpContext.Current.Response.Clear();
          //HttpContext.Current.Response.BinaryWrite(GCheckout.Util.EncodeHelper.Serialize(response));

          foreach (var VARIABLE in data)
          {
              writer.WriteLine(VARIABLE + "\n");            
          }
      }
      catch(Exception ex){
          writer.WriteLine(ex.Message);
      }
      writer.Close();
      Response.StatusCode = 200;
      Response.Write("Successful");
  }
</script>