import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Models/OpportunityImg.dart';

class CreateOpportunityScreen extends StatefulWidget {
  const CreateOpportunityScreen({super.key});

  @override
  _AddOpportunityScreenState createState() => _AddOpportunityScreenState();
}

class _AddOpportunityScreenState extends State<CreateOpportunityScreen> {
  final _formKey = GlobalKey<FormState>();
  String _name = '';
  double _price = 0;
  int _vacancies = 0;
  OppCategory _oppcategory = OppCategory.AGRICULTURA;
  String _description = '';
  Location _location = Location.PORTO;
  String _address = '';
  DateTime _date = DateTime.now();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: LayoutBuilder(
        builder: (context, constraints) {
          if (constraints.maxWidth < 600) {
            // Layout para telas pequenas (smartphones)
            return _buildMobileLayout();
          } else if (constraints.maxWidth < 1200) {
            // Layout para telas médias (tablets)
            return _buildTabletLayout();
          } else {
            // Layout para telas grandes (desktops)
            return _buildDesktopLayout();
          }
        },
      ),
    );
  }

  Widget _buildMobileLayout() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Center(
            child: _buildImagePicker(),
          ),
          const SizedBox(height: 16),
          _buildTextFields(),
          const SizedBox(height: 16),
          _buildDropdowns(),
          const SizedBox(height: 16),
          _buildDateField(),
          const SizedBox(height: 16),
          _buildAddressField(),
          const SizedBox(height: 16),
          _buildSubmitButton(),
        ],
      ),
    );
  }

  Widget _buildTabletLayout() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(flex: 1, child: _buildImagePicker()),
              const SizedBox(width: 16),
              Expanded(flex: 2, child: _buildTextFields()),
            ],
          ),
          const SizedBox(height: 16),
          _buildDropdowns(),
          const SizedBox(height: 16),
          Row(
            children: [
              Expanded(flex: 2, child: _buildDateField()),
              const SizedBox(width: 16),
              Expanded(flex: 1, child: _buildAddressField()),
            ],
          ),
          const SizedBox(height: 16),
          _buildSubmitButton(),
        ],
      ),
    );
  }

  Widget _buildDesktopLayout() {
    return Padding(
      padding: const EdgeInsets.only(left: 400, right: 400, top: 50),
      child: SingleChildScrollView(
        // Adiciona o scroll
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                _buildImagePicker(),
                const SizedBox(width: 16),
                Expanded(child: _buildTextFields()),
              ],
            ),
            const SizedBox(height: 16),
            _buildDropdowns(),
            const SizedBox(height: 16),
            _buildDateField(),
            const SizedBox(height: 16),
            _buildAddressField(),
            const SizedBox(height: 16),
            _buildSubmitButton(),
          ],
        ),
      ),
    );
  }

  Widget _buildImagePicker() {
    return Container(
      width: 300,
      height: 250,
      color: Colors.grey[300],
      child: Center(
        child: ElevatedButton(
          onPressed: () {},
          style: ElevatedButton.styleFrom(backgroundColor: Colors.green),
          child: const Text('Escolher Fotos',
              style: TextStyle(color: Colors.white)),
        ),
      ),
    );
  }

  Widget _buildTextFields() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _buildTextField(
          label: 'Nome da Oportunidade:',
          onSaved: (value) => _name = value!,
          validator: (value) => value == null || value.isEmpty
              ? 'Por favor, insira o nome da oportunidade'
              : null,
        ),
        const SizedBox(height: 16),
        _buildTextField(
          label: 'Descrição:',
          onSaved: (value) => _description = value!,
          validator: (value) => value == null || value.isEmpty
              ? 'Por favor, insira uma descrição da oportunidade'
              : null,
          maxLines: 4, // Definindo o campo para aceitar até 6 linhas visíveis
          keyboardType: TextInputType.multiline, // Necessário para multiline
        ),
        const SizedBox(height: 16),
        Row(
          children: [
            Expanded(
              child: _buildTextField(
                label: 'Preço:',
                keyboardType: TextInputType.numberWithOptions(decimal: true),
                onSaved: (value) => _price = double.parse(value!),
                validator: (value) => value == null || value.isEmpty
                    ? 'Por favor, insira o preço da oportunidade'
                    : null,
              ),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: _buildTextField(
                label: 'Vagas:',
                keyboardType: TextInputType.number,
                onSaved: (value) => _vacancies = int.parse(value!),
                validator: (value) => value == null || value.isEmpty
                    ? 'Por favor, insira a quantidade de vagas'
                    : null,
              ),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildTextField({
    required String label,
    TextInputType keyboardType = TextInputType.text,
    required FormFieldSetter<String> onSaved,
    required FormFieldValidator<String> validator,
    int? maxLines, // Adicionando o parâmetro
  }) {
    return TextFormField(
      decoration: InputDecoration(
        labelText: label,
        alignLabelWithHint: true,
        border: const OutlineInputBorder(),
      ),
      keyboardType: keyboardType,
      validator: validator,
      onSaved: onSaved,
      maxLines: maxLines,
      textAlign: TextAlign
          .start, // Usando maxLines para ajustar a altura do campo // Alinha o texto no topo da caixa
    );
  }

  Widget _buildDropdowns() {
    List<String> categories =
        OppCategory.values.map((category) => category.name).toList();
    List<String> locations =
        Location.values.map((location) => location.name).toList();
    return Column(
      children: [
        _buildDropdown(
          label: 'Categoria:',
          items: categories,
          onChanged: (value) {
            setState(() {
              for (int i = 0; i < categories.length; i++) {
                if (categories[i].toLowerCase() == value?.toLowerCase()) {
                  _oppcategory = categoryFromInt(i);
                }
              }
            });
          },
          validator: (value) => value == null || value.isEmpty
              ? 'Por favor, selecione uma categoria'
              : null,
        ),
        const SizedBox(height: 16),
        _buildDropdown(
          label: 'Localização:',
          items: locations,
          onChanged: (value) {
            setState(() {
              for (int i = 0; i < locations.length; i++) {
                if (locations[i].toLowerCase() == value?.toLowerCase()) {
                  _location = locationFromInt(i);
                }
              }
            });
          },
          validator: (value) => value == null || value.isEmpty
              ? 'Por favor, selecione uma localização'
              : null,
        ),
      ],
    );
  }

  Widget _buildDateField() {
    return TextFormField(
      decoration: const InputDecoration(
        labelText: 'Data:',
        border: OutlineInputBorder(),
      ),
      readOnly: true,
      onTap: () async {
        FocusScope.of(context).requestFocus(FocusNode());
        DateTime? pickedDate = await showDatePicker(
          context: context,
          initialDate: _date,
          firstDate: DateTime(2000),
          lastDate: DateTime(2101),
        );

        if (pickedDate != null) {
          setState(() {
            _date = pickedDate;
          });
        }
      },
      controller: TextEditingController(
        text: '${_date.toLocal()}'.split(' ')[0],
      ),
    );
  }

  Widget _buildAddressField() {
    return _buildTextField(
      label: 'Endereço:',
      onSaved: (value) => _address = value!,
      validator: (value) => value == null || value.isEmpty
          ? 'Por favor, insira o endereço'
          : null,
    );
  }

  Widget _buildDropdown({
    required String label,
    required List<String> items,
    required ValueChanged<String?> onChanged,
    required FormFieldValidator<String> validator,
  }) {
    return DropdownButtonFormField<String>(
      decoration: InputDecoration(
        labelText: label,
        border: const OutlineInputBorder(),
      ),
      items: items
          .map((item) => DropdownMenuItem<String>(
                value: item,
                child: Text(item),
              ))
          .toList(),
      onChanged: onChanged,
      validator: validator,
    );
  }

  Widget _buildSubmitButton() {
    return Center(
      child: ElevatedButton(
        onPressed: () {
          if (_formKey.currentState?.validate() ?? false) {
            // Se o formulário for válido, chama a função de envio
            _handleSubmit();
          }
        },
        child: const Text('Enviar'),
        style: ElevatedButton.styleFrom(
          backgroundColor: Colors.green,
          padding: const EdgeInsets.symmetric(vertical: 20.0, horizontal: 70.0),
        ),
      ),
    );
  }

  void _handleSubmit() {
    // Aqui você pode processar os dados do formulário, como salvar ou enviar para um servidor.
    // Exemplo:
    print("Dados Enviados: ");
    print("Nome: $_name");
    print("Preço: $_price");
    print("Vagas: $_vacancies");
    print("Categoria: $_oppcategory");
    print("Descrição: $_description");
    print("Localização: $_location");
    print("Endereço: $_address");
    print("Data: $_date");
  }
}
