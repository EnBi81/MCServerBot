class SignalRParameterSwagger {

    parameterType;
    subStype;
    valueTd;

    constructor(parameterTr) {
        this.valueTd = parameterTr.querySelector('.parameters-col_description');
        let typeElement = parameterTr.querySelector('.parameter__type');
        let parameterTypeTemp = typeElement.textContent;

        if (parameterTypeTemp.indexOf('(') >= 0) {
            this.parameterType = parameterTypeTemp.substring(0, parameterTypeTemp.indexOf('('));
            this.subStype = parameterTypeTemp.substring(parameterTypeTemp.indexOf('(') + 2);
            this.subStype = this.subStype.substring(0, this.subStype.indexOf(')'))
        }
        else {
            this.parameterType = parameterTypeTemp;
            this.subStype = 'double';
        }
    }

    #getCorrectValue(paramType, obj) {
        if (paramType === 'string') {
            return obj.querySelector('input').value;
        }
        else if (paramType === 'integer') {
            let textValue = obj.querySelector('input').value;
            return isNaN(textValue) ? -1 : parseInt(textValue, 10);
        }
        else if (paramType === 'number') {
            if (this.subStype === 'double') {
                let textValue = obj.querySelector('input').value;
                return isNaN(textValue) ? -1 : parseFloat(textValue);
            }
            else if (this.subStype === 'float') {
                let textValue = obj.querySelector('input').value;
                if (isNaN(textValue))
                    return -1;

                let float64 = parseFloat(textValue);
                let float32View = new DataView(new ArrayBuffer(4));
                float32View.setFloat32(0, float64);

                let float32 = float32View.getFloat32();
                return float32;
            }
            else
                return 0;
        }
        else if (paramType === 'boolean') {
            let value = obj.querySelector('select').value;
            return value === 'true';
        }
        else if (paramType === 'object') {
            let value = obj.querySelector('textarea').value;
            try {
                let parsed = JSON.parse(value);
                return parsed;
            }
            catch (e) {
                return null;
            }
            

        }
        else if (paramType.startsWith('array')) {
            let arrayType = paramType.substring(paramType.indexOf('[') + 1);
            arrayType = arrayType.substring(0, arrayType.indexOf(']'));

            if (arrayType === 'array') {
                return 0;
            }

            let valueObjects = obj.querySelectorAll('.json-schema-form-item');
            let resultArr = [];

            for (const valueObj of valueObjects) {
                let value = this.#getCorrectValue(arrayType, valueObj);
                resultArr.push(value);
            }

            return resultArr;
        }

        return null;
    }
    
    getValue() {
        return this.#getCorrectValue(this.parameterType, this.valueTd);
    }
}