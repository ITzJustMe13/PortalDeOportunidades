import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Services/opportunity_api_handler.dart';
import 'package:frontend/Views/OpportunityManager.dart';
import 'package:intl/intl.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:image_picker/image_picker.dart';
import 'dart:convert';
import 'dart:typed_data';
import 'package:frontend/Models/OpportunityImg.dart';
import 'package:provider/provider.dart';
import 'package:frontend/Components/OpportunityTextField.dart';

class EditOpportunityScreen extends StatefulWidget {
  final Opportunity opportunity;

  const EditOpportunityScreen({super.key, required this.opportunity});

  @override
  _EditOpportunityScreenState createState() => _EditOpportunityScreenState();
}

class _EditOpportunityScreenState extends State<EditOpportunityScreen> {
  late ScrollController verticalScrollController;
  late TextEditingController nameController;
  late TextEditingController descriptionController;
  late TextEditingController addressController;
  late TextEditingController vacanciesController;
  late TextEditingController priceController;

  DateTime? _oppDateController;
  TimeOfDay? _oppTimeController;
  final List<OpportunityImg> _opportunityImgs = [];
  late Location selectedLocation;
  late OppCategory selectedCategory;

  @override
  void initState() {
    super.initState();
    verticalScrollController = ScrollController();

    // Initialize controllers with the current values from the Opportunity object
    nameController = TextEditingController(text: widget.opportunity.name);
    descriptionController = TextEditingController(text: widget.opportunity.description);
    addressController = TextEditingController(text: widget.opportunity.address);
    vacanciesController = TextEditingController(text: widget.opportunity.vacancies.toString());
    priceController = TextEditingController(text: widget.opportunity.price.toString());
    selectedLocation = widget.opportunity.location;
    selectedCategory = widget.opportunity.category;
  }

  @override
  void dispose() {
    verticalScrollController.dispose();
    nameController.dispose();
    addressController.dispose();
    descriptionController.dispose();
    vacanciesController.dispose();
    priceController.dispose();
    super.dispose();
  }

  void _saveOpportunity() async {
    
    final List<OpportunityImg> finalImages = _opportunityImgs.isNotEmpty
      ? _opportunityImgs
      : widget.opportunity.opportunityImgs;

    Opportunity updatedOpportunity = Opportunity(
      opportunityId: widget.opportunity.opportunityId,
      name: nameController.text, 
      price: double.tryParse(priceController.text) ?? 0.0, 
      vacancies: int.tryParse(vacanciesController.text) ?? 0, 
      isActive: widget.opportunity.isActive, 
      category: selectedCategory, 
      description: descriptionController.text, 
      location: selectedLocation, 
      address: addressController.text, 
      userId: widget.opportunity.userId, 
      reviewScore: widget.opportunity.reviewScore, 
      date: (_oppDateController ?? DateTime.now()).add(Duration(days: 1)), 
      isImpulsed: widget.opportunity.isImpulsed, 
      opportunityImgs: finalImages);

    
    final success = await Provider.of<OpportunityApiHandler>(context, listen: false)
      .editOpportunity(widget.opportunity.opportunityId, updatedOpportunity);
    
    if (success) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Alterações gravadas com sucesso!'),
          backgroundColor: Colors.green,
        ),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Falha ao gravar as alterações. Tente novamente!'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: LayoutBuilder(
        builder: (context, constraints) {
          if (constraints.maxWidth < 600) {
            return _buildMobileLayout();
          } else if (constraints.maxWidth < 1200) {
            return _buildTabletLayout();
          } else {
            return _buildDesktopLayout();
          }
        },
      ),
    );
  }

  Widget _buildMobileLayout() {
    return SingleChildScrollView(
      controller: verticalScrollController,
      padding: const EdgeInsets.all(20.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            "Editar a sua Oportunidade:",
            style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
          ),
          SizedBox(height: 20),
          OpportunityTextField(
            label: 'Nome',
            controller: nameController,
          ),
          SizedBox(height: 20),
          OpportunityTextField(
            label: 'Descrição',
            controller: descriptionController,
            maxLines: 3,
          ),
          SizedBox(height: 20),
          OpportunityTextField(
            label: 'Endereço',
            controller: addressController,
          ),
          SizedBox(height: 20),
          Text(
            'Localização',
            style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
          ),
          SizedBox(height: 10),
          DropdownButton<Location>(
            isExpanded: true,
            value: selectedLocation,
            onChanged: (Location? newValue) {
              setState(() {
                selectedLocation = newValue!;
              });
            },
            items: Location.values.map((Location location) {
              return DropdownMenuItem<Location>(
                value: location,
                child: Text(location.name),
              );
            }).toList(),
          ),
          SizedBox(height: 20),
          Text(
            'Categoria',
            style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
          ),
          SizedBox(height: 10),
          DropdownButton<OppCategory>(
            isExpanded: true,
            value: selectedCategory,
            onChanged: (OppCategory? newValue) {
              setState(() {
                selectedCategory = newValue!;
              });
            },
            items: OppCategory.values.map((OppCategory category) {
              return DropdownMenuItem<OppCategory>(
                value: category,
                child: Text(category.name),
              );
            }).toList(),
          ),
          SizedBox(height: 20),
          OpportunityTextField(
            label: 'Vagas',
            controller: vacanciesController,
            maxLines: 1,
            inputType: TextInputType.number, // For numeric input
          ),
          SizedBox(height: 20),
          ElevatedButton(
            child: Text(
              _oppDateController == null
                  ? 'Selecione a Data e Hora da Oportunidade'
                  : '${_oppDateController!.day}/${_oppDateController!.month}/${_oppDateController!.year} ${_oppDateController!.hour}:${_oppDateController!.minute}',
            ),
            onPressed: () => _showDateAndTimePicker(context),
          ),
          SizedBox(height: 20),
          _buildImagesPicker(),
          SizedBox(height: 20),
          ElevatedButton(
            style: ElevatedButton.styleFrom(
              backgroundColor: Color(0xFF50C878), // Set button color here
            ),
            onPressed: () {
              _saveOpportunity(); // Call the save function
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (context) => OpportunityManagerScreen()
                ),
              );
            },
            child: Text(
              'Salvar',
              style: TextStyle(color: Colors.white), // Adjust text color if needed
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildTabletLayout() {
    return _buildMobileLayout();
  }

  Widget _buildDesktopLayout() {
    return _buildMobileLayout();
  }

  Widget _buildImagesPicker() {
    return Container(
      width: 300,
      height: 250,
      color: Colors.grey[300],
      child: Center(
        child: Column(
          children: [
            ElevatedButton(
              style: ElevatedButton.styleFrom(backgroundColor: Colors.green),
              child: const Text('Escolher Fotos',
                  style: TextStyle(color: Colors.white)),
              onPressed: () async {
                final List<XFile> pickedFiles = await ImagePicker()
                    .pickMultiImage(); // Pick multiple images
                if (pickedFiles != null && pickedFiles.isNotEmpty) {
                  final List<String> base64Images = await Future.wait(
                    pickedFiles.map((file) async {
                      final Uint8List fileBytes = await file.readAsBytes();
                      return base64Encode(fileBytes);
                    }),
                  );
                  setState(() {
                    for (var image in base64Images) {
                      var oppImg = OpportunityImg(
                          imgId: 0, opportunityId: 0, imageBase64: image);
                      _opportunityImgs.add(oppImg);
                    }
                  });
                }
              },
            ),
            SizedBox(height: 16.0),
            _opportunityImgs.isNotEmpty
                ? GridView.builder(
                    shrinkWrap: true, // Allow GridView to fit within a Column
                    itemCount: _opportunityImgs.length,
                    gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
                      crossAxisCount: 3, // Number of columns
                      crossAxisSpacing: 8.0,
                      mainAxisSpacing: 8.0,
                    ),
                    itemBuilder: (context, index) {
                      final Uint8List imageBytes = Uint8List.fromList(
                          base64Decode(_opportunityImgs[index].imageBase64));
                      return Image.memory(
                        imageBytes,
                        width: 100,
                        height: 100,
                        fit: BoxFit.cover,
                      );
                    },
                  )
                : Center(child: Text('Nenhuma imagem selecionada')),
          ],
        ),
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
        _oppDateController = DateTime(
          pickedDate.year,
          pickedDate.month,
          pickedDate.day,
          pickedTime.hour,
          pickedTime.minute,
        );
        _oppTimeController = pickedTime;
      });
    }
  }
}

}





