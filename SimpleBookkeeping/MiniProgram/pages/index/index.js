Page({
  data: {
    balance: 0,
    totalIncome: 0,
    totalExpense: 0,
    outstanding: 0,
    unpaidCount: 0
  },

  onLoad() {
    this.loadData();
  },

  onShow() {
    this.loadData();
  },

  loadData() {
    const app = getApp();
    const userId = app.globalData.userId;
    const apiUrl = app.globalData.apiUrl;

    // 加载账本汇总
    wx.request({
      url: `${apiUrl}/transactions/summary?userId=${userId}`,
      success: (res) => {
        if (res.statusCode === 200) {
          this.setData({
            totalIncome: res.data.totalIncome,
            totalExpense: res.data.totalExpense,
            balance: res.data.balance
          });
        }
      }
    });

    // 加载赊账汇总
    wx.request({
      url: `${apiUrl}/credits/summary?userId=${userId}`,
      success: (res) => {
        if (res.statusCode === 200) {
          this.setData({
            outstanding: res.data.outstanding,
            unpaidCount: res.data.unpaidCount
          });
        }
      }
    });
  },

  goToAddTransaction() {
    wx.navigateTo({
      url: '/pages/transactions/transactions?action=add'
    });
  },

  goToAddCredit() {
    wx.navigateTo({
      url: '/pages/credits/credits?action=add'
    });
  }
})
