--User_GetOrdersHistory 1
Alter PROCEDURE User_GetOrdersHistory
    @UserId int = 1
AS
BEGIN

select * into #TempOrders from Orders where [User_ID] = @UserId

select * from #TempOrders

select StoreOrders.* into #TempStoreOrders from StoreOrders
join #TempOrders on #TempOrders.Id = StoreOrders.Order_Id

select * from #TempStoreOrders

select Order_Items.Qty, Products.* from Order_Items
join #TempStoreOrders on #TempStoreOrders.Id = Order_Items.StoreOrder_Id
join Products on Products.Id = Order_Items.Product_Id


IF OBJECT_ID('tempdb..#TempOrders') IS NOT NULL Drop Table #TempOrders

IF OBJECT_ID('tempdb..#TempStoreOrders') IS NOT NULL Drop Table #TempStoreOrders

END
