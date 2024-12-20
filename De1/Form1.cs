using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace De1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            using (var context = new Model1())
            {
                // Load danh sách lớp vào ComboBox
                var lops = context.Lops.ToList();
                cbblophoc.DataSource = lops;
                cbblophoc.DisplayMember = "TenLop"; // Hiển thị tên lớp
                cbblophoc.ValueMember = "MaLop";   // Giá trị là mã lớp

                // Load danh sách sinh viên vào DataGridView
                LoadSinhVien();
            }
        }
       

        private void LoadSinhVien()
        {
            using (var context = new Model1())
            {
                var sinhvienList = context.Sinhviens.Include("Lop").Select(sv => new
                {
                    MaSV = sv.MaSV,
                    HoTen = sv.HoTenSV,
                    NgaySinh = sv.NgaySinh,
                    Lop = sv.Lop.TenLop
                }).ToList();

                dgvSinhvien.DataSource = sinhvienList;
            }
        }

        private void cbblophoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (var context = new Model1())
            {
                var selectedMaLop = cbblophoc.SelectedValue.ToString();

                var sinhvienList = context.Sinhviens
                    .Where(sv => sv.MaLop == selectedMaLop)
                    .Include("Lop")
                    .Select(sv => new
                    {
                        MaSV = sv.MaSV,
                        HoTen = sv.HoTenSV,
                        NgaySinh = sv.NgaySinh,
                        Lop = sv.Lop.TenLop
                    }).ToList();

                dgvSinhvien.DataSource = sinhvienList;
            }
        }

        private void btnthem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new Model1())
                {
                    // Kiểm tra xem mã sinh viên đã tồn tại chưa
                    var maSV = txtmasv.Text;
                    if (context.Sinhviens.Any(sv => sv.MaSV == maSV))
                    {
                        MessageBox.Show("Mã sinh viên đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Tạo đối tượng Sinhvien mới
                    var sinhVien = new Sinhvien
                    {
                        MaSV = txtmasv.Text,
                        HoTenSV = txthoten.Text,
                        NgaySinh = dtngaysinh.Value,
                        MaLop = cbblophoc.SelectedValue.ToString()
                    };

                    // Thêm sinh viên vào DbSet
                    context.Sinhviens.Add(sinhVien);
                    context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                    // Tải lại dữ liệu hiển thị
                    LoadSinhVien();
                    MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnsua_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new Model1())
                {
                    // Lấy mã sinh viên từ TextBox
                    var maSV = txtmasv.Text;

                    // Tìm sinh viên trong cơ sở dữ liệu
                    var sinhVien = context.Sinhviens.FirstOrDefault(sv => sv.MaSV == maSV);

                    if (sinhVien != null)
                    {
                        // Cập nhật thông tin sinh viên
                        sinhVien.HoTenSV = txthoten.Text;
                        sinhVien.NgaySinh = dtngaysinh.Value;
                        sinhVien.MaLop = cbblophoc.SelectedValue.ToString();

                        context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                        // Tải lại dữ liệu hiển thị
                        LoadSinhVien();
                        MessageBox.Show("Cập nhật sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new Model1())
                {
                    // Lấy mã sinh viên từ TextBox
                    var maSV = txtmasv.Text;

                    // Tìm sinh viên trong cơ sở dữ liệu
                    var sinhVien = context.Sinhviens.FirstOrDefault(sv => sv.MaSV == maSV);

                    if (sinhVien != null)
                    {
                        // Xóa sinh viên khỏi DbSet
                        context.Sinhviens.Remove(sinhVien);
                        context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                        // Tải lại dữ liệu hiển thị
                        LoadSinhVien();
                        MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvSinhvien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btntimkiem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new Model1())
                {
                    // Lấy giá trị tìm kiếm từ TextBox
                    var keyword = txtTimKiem.Text.Trim();

                    if (string.IsNullOrWhiteSpace(keyword))
                    {
                        MessageBox.Show("Vui lòng nhập từ khóa để tìm kiếm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Tìm kiếm sinh viên theo MaSV hoặc HoTenSV
                    var result = context.Sinhviens.Include("Lop")
                                  .Where(sv => sv.MaSV.Contains(keyword) || sv.HoTenSV.Contains(keyword))
                                  .Select(sv => new
                                  {
                                      MaSV = sv.MaSV,
                                      HoTen = sv.HoTenSV,
                                      NgaySinh = sv.NgaySinh,
                                      Lop = sv.Lop.TenLop
                                  })
                                  .ToList();

                    // Kiểm tra nếu không có kết quả
                    if (result.Count == 0)
                    {
                        MessageBox.Show("Không tìm thấy sinh viên nào phù hợp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // Hiển thị kết quả lên DataGridView
                    dgvSinhvien.DataSource = result;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnthoat_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Bạn có chắc chắn muốn thoát không?",
                                  "Xác nhận",
                                  MessageBoxButtons.YesNo,
                                  MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                this.Close(); // Đóng form hiện tại
            }
        }
    }
}
    

