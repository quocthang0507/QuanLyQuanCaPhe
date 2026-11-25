# QuanLyQuanCaPhe
> Ứng dụng Quản lý quán cà phê quy mô vừa - Môn học Lập trình cơ sở dữ liệu
Sử dụng mô hình EntityFramework

## Cài đặt và nâng cấp

- Windows, .NET 10 SDK (hoặc SDK mới hơn hỗ trợ `net10.0-windows`) và SQL Server 2016 SP1 trở lên hoặc SQL Server LocalDB.
- Ứng dụng tiếp tục sử dụng Entity Framework 6 và cơ sở dữ liệu SQL Server hiện có.
- **Sao lưu cơ sở dữ liệu trước khi nâng cấp.** Chọn đúng cơ sở dữ liệu `CafeRestaurant`, sau đó chạy lần lượt [migrate-order-uniqueness.sql](QuanLyQuanCafe/migrate-order-uniqueness.sql) và [migrate-identifiers.sql](QuanLyQuanCafe/migrate-identifiers.sql). Không chạy các tập lệnh tạo cơ sở dữ liệu mới trên dữ liệu đang sử dụng.
- Với cơ sở dữ liệu mới, tạo dữ liệu bằng [SQL Script.sql](SQL%20Script.sql), rồi chạy hai tập lệnh nâng cấp trên. [Full Script.sql](Full%20Script.sql) là bản xuất cũ, có thể chứa đường dẫn máy cũ và ràng buộc không còn phù hợp.
- Chỉnh chuỗi kết nối `CafeRestaurantEntities` trong [App.config](QuanLyQuanCafe/QuanLyQuanCafe/App.config) cho phù hợp máy chủ trước khi build; cấu hình mặc định dùng `(localdb)\MSSQLLocalDB`. Không đưa mật khẩu vào mã nguồn hoặc kho Git.
- Build bằng `dotnet build QuanLyQuanCafe\QuanLyQuanCafe.sln -c Release`. Cấu hình trong thư mục xuất bản là `QuanLyQuanCafe.dll.config`.

Các thay đổi gồm sửa nhập món vào lưới, lưu hóa đơn và chi tiết trong cùng giao dịch, kiểm tra khuyến mãi, báo cáo theo ngày hóa đơn và tính lại mã trong ngày. Mã cũ có giới hạn 9.999 hóa đơn và 9.999 dòng đơn hàng mỗi ngày; ứng dụng sẽ báo lỗi khi hết mã.

## Giới hạn hiện tại

- Chỉ hỗ trợ một cửa hàng, một máy; dữ liệu đơn đang nhập và trạng thái bàn trên giao diện chưa được khôi phục sau khi đóng ứng dụng.
- Chuyển bàn vẫn chưa được triển khai; nút “in hóa đơn” hiện hiển thị thông tin trên màn hình, chưa điều khiển máy in.
- Các thay đổi cần được kiểm thử với bản sao dữ liệu và quy trình thực tế của cửa hàng trước khi sử dụng chính thức.

![Diagram](Diagram.PNG?raw=true "Diagram")

![Login Form](Capture-1.PNG?raw=true "Login Form")

![Main Form](Capture-2.PNG?raw=true "Main Form")

![Drink Form](Capture-3.PNG?raw=true "Drink Form")

## Tác giả

Nếu có góp ý, đề xuất, vui lòng liên hệ email sau:
La Quốc Thắng - quocthang_0507@yahoo.com.vn