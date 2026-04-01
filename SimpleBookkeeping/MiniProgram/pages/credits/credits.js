Page({
  data: {
    currentTab: 'list',
    credits: [],
    formData: {
      customerName: '',
      customerPhone: '',
      amount: '',
      description: '',
      date: new Date().toISOString().split('T')[0],
      dueDate: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString().split('T')[0]
    }
  },

  onLoad(options) {
    if (options.action === 'add') {
      this.setData({ currentTab: 'add' });
    }
    this.loadCredits();
  },

  onShow() {
    if (this.data.currentTab === 'list') {
      this.loadCredits();
    }
  },

  switchTab(e) {
    const tab = e.currentTarget.dataset.tab;
    this.setData({ currentTab: tab });
    if (tab === 'list') {
      this.loadCredits();
    }
  },

  loadCredits() {
    const app = getApp();
    const userId = app.globalData.userId;
    const apiUrl = app.globalData.apiUrl;

    wx.request({
      url: `${apiUrl}/credits?userId=${userId}`,
      success: (res) => {
        if (res.statusCode === 200) {
          const credits = res.data.map(c => ({
            ...c,
            date: c.date.split('T')[0]
          }));
          this.setData({ credits });
        }
      }
    });
  },

  onNameInput(e) {
    this.setData({ 'formData.customerName': e.detail.value });
  },

  onPhoneInput(e) {
    this.setData({ 'formData.customerPhone': e.detail.value });
  },

  onAmountInput(e) {
    this.setData({ 'formData.amount': e.detail.value });
  },

  onDescInput(e) {
    this.setData({ 'formData.description': e.detail.value });
  },

  onDateChange(e) {
    this.setData({ 'formData.date': e.detail.value });
  },

  onDueDateChange(e) {
    this.setData({ 'formData.dueDate': e.detail.value });
  },

  submitCredit() {
    const { formData } = this.data;
    
    if (!formData.customerName) {
      wx.showToast({ title: '请输入客户姓名', icon: 'none' });
      return;
    }
    
    if (!formData.amount || parseFloat(formData.amount) <= 0) {
      wx.showToast({ title: '请输入金额', icon: 'none' });
      return;
    }

    const app = getApp();
    const userId = app.globalData.userId;
    const apiUrl = app.globalData.apiUrl;

    const payload = {
      userId,
      customerName: formData.customerName,
      customerPhone: formData.customerPhone,
      amount: parseFloat(formData.amount),
      description: formData.description,
      date: formData.date,
      dueDate: formData.dueDate,
      status: 'unpaid',
      paidAmount: 0
    };

    wx.request({
      url: `${apiUrl}/credits`,
      method: 'POST',
      data: payload,
      success: (res) => {
        if (res.statusCode === 201) {
          wx.showToast({ title: '保存成功', icon: 'success' });
          this.setData({
            currentTab: 'list',
            formData: {
              customerName: '',
              customerPhone: '',
              amount: '',
              description: '',
              date: new Date().toISOString().split('T')[0],
              dueDate: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString().split('T')[0]
            }
          });
          this.loadCredits();
        } else {
          wx.showToast({ title: '保存失败', icon: 'none' });
        }
      },
      fail: () => {
        wx.showToast({ title: '网络错误', icon: 'none' });
      }
    });
  },

  recordPayment(e) {
    const creditId = e.currentTarget.dataset.id;
    const credit = this.data.credits.find(c => c.id === creditId);
    
    if (!credit) return;

    const remaining = credit.amount - credit.paidAmount;
    
    wx.showModal({
      title: '记录收款',
      editable: true,
      placeholderText: `最多可收¥${remaining}`,
      success: (res) => {
        if (res.confirm && res.content) {
          const amount = parseFloat(res.content);
          if (isNaN(amount) || amount <= 0 || amount > remaining) {
            wx.showToast({ title: '请输入有效金额', icon: 'none' });
            return;
          }

          const app = getApp();
          const userId = app.globalData.userId;
          const apiUrl = app.globalData.apiUrl;

          wx.request({
            url: `${apiUrl}/credits/${creditId}/payment?userId=${userId}`,
            method: 'POST',
            data: { amount },
            header: { 'Content-Type': 'application/json' },
            success: (res) => {
              if (res.statusCode === 200) {
                wx.showToast({ title: '收款成功', icon: 'success' });
                this.loadCredits();
              } else {
                wx.showToast({ title: '操作失败', icon: 'none' });
              }
            },
            fail: () => {
              wx.showToast({ title: '网络错误', icon: 'none' });
            }
          });
        }
      }
    });
  }
})
