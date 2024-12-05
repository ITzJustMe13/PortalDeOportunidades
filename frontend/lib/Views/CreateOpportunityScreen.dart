import 'dart:convert';

import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Components/OpportunityImagePicker.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Models/OpportunityImg.dart';
import 'package:frontend/State/CreateOpportunityState.dart';
import 'package:frontend/Views/OpportunityDetailsScreen.dart';
import 'package:image_picker/image_picker.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

class CreateOpportunityScreen extends StatefulWidget {
  const CreateOpportunityScreen({super.key});

  @override
  State<CreateOpportunityScreen> createState() => _AddOpportunityScreenState();
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
  String _errorMessage = "";
  List<OpportunityImg> _opportunityImgs = [];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: CustomAppBar(),
        endDrawer: CustomDrawer(),
        body: Consumer<CreateOpportunityState>(
          builder: (context, createOpportunityState, child) => LayoutBuilder(
            builder: (context, constraints) {
              if (constraints.maxWidth < 600) {
                // Layout para telas pequenas (smartphones)
                return _buildMobileLayout(createOpportunityState);
              } else if (constraints.maxWidth < 1200) {
                // Layout para telas médias (tablets)
                return _buildTabletLayout(createOpportunityState);
              } else {
                // Layout para telas grandes (desktops)
                return _buildDesktopLayout(createOpportunityState);
              }
            },
          ),
        ));
  }

  Widget _buildMobileLayout(CreateOpportunityState createOpportunityState) {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16.0),
      child: Form(
        key: _formKey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Center(
              child: OpportunityImagePicker(
                opportunityImgs: _opportunityImgs,
                onImagesChanged: (newImages) {
                  setState(() {
                    _opportunityImgs = newImages;
                  });
                },
              ),
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
            _buildSubmitButton(createOpportunityState),
          ],
        ),
      ),
    );
  }

  Widget _buildTabletLayout(CreateOpportunityState createOpportunityState) {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16.0),
      child: Form(
        key: _formKey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  flex: 1,
                  child: OpportunityImagePicker(
                    opportunityImgs: _opportunityImgs,
                    onImagesChanged: (newImages) {
                      setState(() {
                        _opportunityImgs = newImages;
                      });
                    },
                  ),
                ),
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
            _buildSubmitButton(createOpportunityState),
          ],
        ),
      ),
    );
  }

  Widget _buildDesktopLayout(CreateOpportunityState createOpportunityState) {
    return Padding(
      padding: const EdgeInsets.only(left: 400, right: 400, top: 50),
      child: SingleChildScrollView(
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              OpportunityImagePicker(
                opportunityImgs: _opportunityImgs,
                onImagesChanged: (newImages) {
                  setState(() {
                    _opportunityImgs = newImages;
                  });
                },
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
              _buildSubmitButton(createOpportunityState),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildTextField({
    required String label,
    TextInputType keyboardType = TextInputType.text,
    required FormFieldSetter<String> onChanged,
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
      onChanged: onChanged,
      maxLines: maxLines,
      textAlign: TextAlign
          .start, // Usando maxLines para ajustar a altura do campo // Alinha o texto no topo da caixa
    );
  }

  Widget _buildTextFields() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _buildTextField(
            label: 'Nome da Oportunidade:',
            onChanged: (value) => _name = value ?? "",
            validator: (value) {
              if (value == null || value.isEmpty) {
                return 'Por favor, insira o nome da oportunidade';
              } else if (value.length > 100) {
                return 'O nome não deve ter mais do que 100 carateres';
              } else {
                return null;
              }
            }),
        const SizedBox(height: 16),
        _buildTextField(
          label: 'Descrição:',
          onChanged: (value) => _description = value ?? "",
          validator: (value) {
            if (value == null || value.isEmpty) {
              return 'Por favor, insira a descrição da oportunidade';
            } else if (value.length > 100) {
              return 'A descrição não deve ter mais do que 1000 carateres';
            } else {
              return null;
            }
          },
          maxLines: 4, // Definindo o campo para aceitar até 6 linhas visíveis
          keyboardType: TextInputType.multiline, // Necessário para multiline
        ),
        const SizedBox(height: 16),
        Row(
          children: [
            Expanded(
              child: _buildTextField(
                label: 'Preço:(irá ser imposta posteriormente a taxa da plataforma)',
                keyboardType: TextInputType.numberWithOptions(decimal: true),
                onChanged: (value) => _price = double.parse(value ?? ""),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Por favor, insira a descrição da oportunidade';
                  } else if (double.parse(value) < 0) {
                    return 'Deve ser maior que 0';
                  } else {
                    return null;
                  }
                },
              ),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: _buildTextField(
                label: 'Vagas:',
                keyboardType: TextInputType.number,
                onChanged: (value) => _vacancies = int.parse(value ?? ""),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Por favor, insira a descrição da oportunidade';
                  } else if (double.parse(value) < 1 ||
                      double.parse(value) > 30) {
                    return 'Deve ser entre 1 e 30';
                  } else {
                    return null;
                  }
                },
              ),
            ),
          ],
        ),
      ],
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
        _showDateAndTimePicker(context);
      },
      controller: TextEditingController(
        text: DateFormat('yyyy-MM-dd HH:mm:ss').format(_date),
      ),
    );
  }

  void _showDateAndTimePicker(BuildContext context) async {
    final DateTime? pickedDate = await showDatePicker(
      context: context,
      initialDate: DateTime.now(),
      firstDate: DateTime.now(),
      lastDate: DateTime(2100),
    );

    if (pickedDate != null) {
      final TimeOfDay? pickedTime = await showTimePicker(
        context: context,
        initialTime: TimeOfDay.now(),
      );

      if (pickedTime != null) {
        setState(() {
          _date = DateTime(
            pickedDate.year,
            pickedDate.month,
            pickedDate.day,
            pickedTime.hour,
            pickedTime.minute,
          );
        });
      }
    }
  }

  Widget _buildAddressField() {
    return _buildTextField(
      label: 'Endereço:',
      onChanged: (value) => _address = value ?? "",
      validator: (value) {
        if (value == null || value.isEmpty) {
          return 'Por favor, insira a morada da oportunidade';
        } else if (value.length > 200) {
          return 'A morada não deve ter mais do que 200 carateres';
        } else {
          return null;
        }
      },
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

  Widget _buildSubmitButton(CreateOpportunityState createOpportunityState) {
    return Center(
        child: Column(children: [
      if (createOpportunityState.isLoading)
        CircularProgressIndicator()
      else
        ElevatedButton(
          onPressed: () async {
            if (_formKey.currentState!.validate()) {
              _errorMessage = "";
              Opportunity? createdOpportunity =
                  await _handleSubmit(createOpportunityState);
              if (createdOpportunity != null) {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => OpportunityDetailsScreen(
                        opportunity: createdOpportunity),
                  ),
                );
              }
            }
          },
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.green,
            padding:
                const EdgeInsets.symmetric(vertical: 20.0, horizontal: 70.0),
          ),
          child: const Text('Enviar'),
        ),
      if (_errorMessage.isNotEmpty ||
          createOpportunityState.errorMessage != null ||
          createOpportunityState.errorMessage != '')
        Text(
          _errorMessage == ""
              ? createOpportunityState.errorMessage ?? ""
              : _errorMessage,
          style: TextStyle(color: Colors.red),
        ),
    ]));
  }

  Future<Opportunity?> _handleSubmit(
      CreateOpportunityState createOpportunityState) async {
    int currentUserId = await createOpportunityState.getCurrentUserId();

    if (_opportunityImgs.isEmpty) {
      setState(() {
        _errorMessage = 'A oportunidade deve conter imagens';
      });
      return null;
    }

    if (currentUserId == -1) {
      setState(() {
        _errorMessage = 'Erro ao criar conta';
      });
      return null;
    }

    if (_opportunityImgs.length > 5) {
      setState(() {
        _errorMessage = 'Não é permitido adicionar mais de 5 imagens';
      });
      return null;
    }

    final opportunity = Opportunity(
        opportunityId: 0,
        userId: currentUserId,
        name: _name,
        price: _price,
        vacancies: _vacancies,
        isActive: true,
        category: _oppcategory,
        description: _description,
        location: _location,
        address: _address,
        date: _date,
        isImpulsed: false,
        reviewScore: 0,
        opportunityImgs: []);

    return await createOpportunityState.createOpportunity(
        opportunity, _opportunityImgs);
  }
}
