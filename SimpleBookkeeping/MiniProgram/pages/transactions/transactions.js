Page({
  data: {
    currentTab: 'list',
    transactions: [],
    formData: {
      type: 'expense',
      amount: '',
      category: '餐饮',
      description: '',
      date: new Date().toISOString().split('T')[0]
    },
    categoryIndex: 0,
    categories: ['餐饮', '交通', '购物', '娱乐', '医疗', '教育', '居住', '其他']
  },

  onLoad(options) {
    if (options.action === 'add') {
      this.setData({ currentTab: 'add' });
    }
    this.loadTransactions();
  },

  onShow() {
    if (this.data.currentTab === 'list') {
      this.loadTransactions();
    }
  },

  switchTab(e) {
    const tab = e.currentTarget.dataset.tab;
    this.setData({ currentTab: tab });
    if (tab === 'list') {
      this.loadTransactions();
    }
  },

  loadTransactions() {
    const app = getApp();
    const userId = app.globalData.userId;
    const apiUrl = app.globalData.apiUrl;

    wx.request({
      url: `${apiUrl}/transactions?userId=${userId}`,
      success: (res) => {
        if (res.statusCode === 200) {
          const transactions = res.data.map(t => ({
            ...t,
            date: t.date.split('T')[0],
            category: t.category || '其他'
          }));
          this.setData({ transactions });
        }
      }
    });
  },

  selectType(e) {
    const type = e.currentTarget.dataset.type;
    this.setData({ 'formData.type': type });
  },

  onAmountInput(e) {
    this.setData({ 'formData.amount': e.detail.value });
  },

  onCategoryChange(e) {
    const index = e.detail.value;
    this.setData({
      categoryIndex: index,
      'formData.category': this.data.categories[index]
    });
  },

  onDescInput(e) {
    this.setData({ 'formData.description': e.detail.value });
  },

  onDateChange(e) {
    this.setData({ 'formData.date': e.detail.value });
  },

  submitTransaction() {
    const { formData } = this.data;
    
    if (!formData.amount || parseFloat(formData.amount) <= 0) {
      wx.showToast({ title: '请输入金额', icon: 'none' });
      return;
    }

    const app = getApp();
    const userId = app.globalData.userId;
    const apiUrl = app.globalData.apiUrl;

    const payload = {
      userId,
      amount: parseFloat(formData.amount),
      type: formData.type,
      category: formData.category,
      description: formData.description,
      date: formData.date
    };

    wx.request({
      url: `${apiUrl}/transactions`,
      method: 'POST',
      data: payload,
      success: (res) => {
        if (res.statusCode === 201) {
          wx.showToast({ title: '保存成功', icon: 'success' });
          this.setData({
            currentTab: 'list',
            formData: {
              type: 'expense',
              amount: '',
              category: '餐饮',
              description: '',
              date: new Date().toISOString().split('T')[0]
            },
            categoryIndex: 0
          });
          this.loadTransactions();
        } else {
          wx.showToast({ title: '保存失败', icon: 'none' });
        }
      },
      fail: () => {
        wx.showToast({ title: '网络错误', icon: 'none' });
      }
    });
  }
})
