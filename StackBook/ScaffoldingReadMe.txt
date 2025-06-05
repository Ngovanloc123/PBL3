Scaffolding has generated all the files and added the required dependencies.

However the Application's Startup code may require additional changes for things to work end to end.
Add the following code to the Configure method in your Application's Startup class if not already done:

        app.UseEndpoints(endpoints =>
        {
          endpoints.MapControllerRoute(
            name : "areas",
            pattern : "{area:exists}/{controller=Home}/{action=Index}/{id?}"
          );
        });
        
Them gium toi DB 2 cau nay:
ALTER TABLE [StackBook].[dbo].[Orders]
ADD CreatedAt DATETIME NOT NULL 
CONSTRAINT DF_Orders_CreatedAt DEFAULT GETDATE();

ALTER TABLE Reviews ADD OrderId UNIQUEIDENTIFIER NULL;