const utils = {
    _keyPrefix: 'de50da11c12ce3ceef04bbeda5908202',
    setVal: (key, val) => {
        localStorage.setItem(this._keyPrefix + "_" + key, JSON.stringify(val))
    },
    getVal: (key, json = true) => {
        var res = localStorage.getItem(this._keyPrefix + "_" + key)
        if (json && res) {
            res = JSON.parse(res)
        }
        return res
    },
    removeVal: (key) => {
        localStorage.removeItem(this._keyPrefix + "_" + key)
    },
    toInt: (val, defaultVal = 0) => {
        if (!val) return defaultVal
        return Number(val)
    },
    toString: (val, defaultVal = "") => {
        if (!val) return defaultVal
        return String(val)
    },
    toBool: (val, defaultVal = false) => {
        if (!val) return defaultVal
        if (val === "true") return true
        if (val === "True") return true
        if (val === "false") return false
        if (val === "False") return false
        return defaultVal
    }
}